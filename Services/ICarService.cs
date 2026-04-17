using AutoSalon.Models;
using AutoSalon.ViewModels;

namespace AutoSalon.Services;

public interface ICarService
{
    Task<List<Car>> GetAllAsync(CatalogFilterViewModel? filter = null);
    Task<Car?> GetBySlugAsync(string slug);
    Task<Car?> GetByIdAsync(int id);
    Task<int> CreateAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(int id);
    Task IncrementViewAsync(int id);
}