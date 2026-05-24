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
    private const int MaxPhotosPerCar = 20;

    // Папка вне wwwroot — недоступна напрямую через HTTP
    private string UploadsRoot => Path.Combine(_env.ContentRootPath, "App_Data", "uploads");

    public PhotoService(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    private static bool IsAllowedImage(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var header = new byte[4];
        if (stream.Read(header, 0, 4) < 4) return false;

        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) return true; // JPEG
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) return true; // PNG
        if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46) return true; // WebP
        if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) return true; // GIF

        return false;
    }

    public async Task<List<CarPhoto>> SavePhotosAsync(IEnumerable<IFormFile> files, int carId)
    {
        var uploadsPath = Path.Combine(UploadsRoot, carId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var existingCount = await _db.CarPhotos.CountAsync(p => p.CarId == carId);
        var result = new List<CarPhoto>();
        var sortOrder = existingCount;

        foreach (var file in files.ToList())
        {
            if (file.Length == 0) continue;
            if (file.Length > MaxFileSize) continue;
            if (existingCount + result.Count >= MaxPhotosPerCar) break;
            if (!IsAllowedImage(file)) continue;

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
            catch
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

        // Удаляем физический файл из новой папки
        var relativePath = photo.FilePath.TrimStart('/'); // "uploads/5/abc.webp"
        var fullPath = Path.Combine(UploadsRoot,
            relativePath.Replace("uploads/", "").Replace("uploads\\", ""));
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