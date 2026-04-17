using AutoSalon.Models;
using AutoSalon.ViewModels;

namespace AutoSalon.Services;

public interface ICarService
{
    Task<(List<Car> Items, int Total)> GetFilteredAsync(CarFilterViewModel filter);
    Task<List<string>> GetBrandsAsync();
    Task<Car?> GetBySlugAsync(string slug);
    Task<Car?> GetByIdAsync(int id);
    Task<List<Car>> GetByIdsAsync(IEnumerable<int> ids);
    Task<List<Car>> GetSimilarAsync(int carId, string brand, int count = 4);
    Task<int> CreateAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(int id);
    Task IncrementViewAsync(int id);
    Task<int> GetTotalCountAsync();
}