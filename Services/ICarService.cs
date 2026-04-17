using AutoSalon.Models;
using AutoSalon.ViewModels;

namespace AutoSalon.Services;

public interface ICarService
{
    Task<List<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(int id);
    Task<Car?> GetBySlugAsync(string slug);
    Task<(List<Car> Items, int TotalCount)> GetFilteredAsync(CarFilterViewModel filter);
    Task<int> CreateAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(int id);
    Task IncrementViewAsync(int id);
    Task<List<string>> GetBrandsAsync();
}