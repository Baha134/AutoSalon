using AutoSalon.Models;

namespace AutoSalon.Services;

public interface IPhotoService
{
    /// <summary>Сохраняет файлы, конвертирует в WebP, возвращает список CarPhoto</summary>
    Task<List<CarPhoto>> SavePhotosAsync(IEnumerable<IFormFile> files, int carId);
    Task DeletePhotoAsync(int photoId);
    Task SetMainPhotoAsync(int photoId, int carId);
    Task ReorderAsync(int carId, List<int> orderedIds);
}