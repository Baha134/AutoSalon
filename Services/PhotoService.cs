using AutoSalon.Data;
using AutoSalon.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace AutoSalon.Services;

public class PhotoService : IPhotoService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private const int MaxWidth = 1200;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 МБ

    public PhotoService(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<List<CarPhoto>> SavePhotosAsync(IEnumerable<IFormFile> files, int carId)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", carId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var existingCount = await _db.CarPhotos.CountAsync(p => p.CarId == carId);
        var result = new List<CarPhoto>();
        var sortOrder = existingCount;

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            // Проверка размера файла
            if (file.Length > MaxFileSize) continue;

            // Проверка MIME-типа
            if (!allowedTypes.Contains(file.ContentType.ToLower())) continue;

            try
            {
                var fileName = $"{Guid.NewGuid():N}.webp";
                var filePath = Path.Combine(uploadsPath, fileName);

                using var stream = file.OpenReadStream();
                using var image = await Image.LoadAsync(stream);

                if (image.Width > MaxWidth)
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(MaxWidth, 0),
                        Mode = ResizeMode.Max
                    }));

                await image.SaveAsync(filePath, new WebpEncoder { Quality = 82 });

                var photo = new CarPhoto
                {
                    CarId = carId,
                    FilePath = $"/uploads/{carId}/{fileName}",
                    IsMain = sortOrder == 0,
                    SortOrder = sortOrder++
                };

                _db.CarPhotos.Add(photo);
                result.Add(photo);
            }
            catch (UnknownImageFormatException)
            {
                continue;
            }
            catch (Exception)
            {
                continue;
            }
        }

        await _db.SaveChangesAsync();
        return result;
    }

    public async Task DeletePhotoAsync(int photoId)
    {
        var photo = await _db.CarPhotos.FindAsync(photoId);
        if (photo == null) return;

        var fullPath = Path.Combine(_env.WebRootPath, photo.FilePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);

        _db.CarPhotos.Remove(photo);
        await _db.SaveChangesAsync();

        if (photo.IsMain)
        {
            var next = await _db.CarPhotos
                .Where(p => p.CarId == photo.CarId)
                .OrderBy(p => p.SortOrder)
                .FirstOrDefaultAsync();
            if (next != null)
            {
                next.IsMain = true;
                await _db.SaveChangesAsync();
            }
        }
    }

    public async Task SetMainPhotoAsync(int photoId, int carId)
    {
        await _db.CarPhotos
            .Where(p => p.CarId == carId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsMain, false));

        var photo = await _db.CarPhotos.FindAsync(photoId);
        if (photo != null)
        {
            photo.IsMain = true;
            await _db.SaveChangesAsync();
        }
    }

    public async Task ReorderAsync(int carId, List<int> orderedIds)
    {
        var photos = await _db.CarPhotos.Where(p => p.CarId == carId).ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var photo = photos.FirstOrDefault(p => p.Id == orderedIds[i]);
            if (photo != null) photo.SortOrder = i;
        }
        await _db.SaveChangesAsync();
    }
}