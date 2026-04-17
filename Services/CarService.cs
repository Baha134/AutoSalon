using AutoSalon.Data;
using AutoSalon.Models;
using AutoSalon.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Services;

public class CarService : ICarService
{
    private readonly AppDbContext _db;

    public CarService(AppDbContext db) => _db = db;

    public async Task<List<Car>> GetAllAsync() =>
        await _db.Cars.Include(c => c.Photos).OrderByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<Car?> GetByIdAsync(int id) =>
        await _db.Cars.Include(c => c.Photos).Include(c => c.Badge).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Car?> GetBySlugAsync(string slug) =>
        await _db.Cars.Include(c => c.Photos).Include(c => c.Badge).FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<(List<Car> Items, int TotalCount)> GetFilteredAsync(CarFilterViewModel filter)
    {
        var query = _db.Cars.Include(c => c.Photos).Where(c => c.IsActive && c.Status == CarStatus.Active).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Brand))
            query = query.Where(c => c.Brand == filter.Brand);

        if (filter.PriceMin.HasValue)
            query = query.Where(c => c.Price >= filter.PriceMin.Value);

        if (filter.PriceMax.HasValue)
            query = query.Where(c => c.Price <= filter.PriceMax.Value);

        if (filter.YearMin.HasValue)
            query = query.Where(c => c.Year >= filter.YearMin.Value);

        if (filter.YearMax.HasValue)
            query = query.Where(c => c.Year <= filter.YearMax.Value);

        int total = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<int> CreateAsync(Car car)
    {
        car.Slug = GenerateSlug(car);
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
        return car.Id;
    }

    public async Task UpdateAsync(Car car)
    {
        _db.Cars.Update(car);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var car = await _db.Cars.FindAsync(id);
        if (car != null) { _db.Cars.Remove(car); await _db.SaveChangesAsync(); }
    }

    public async Task IncrementViewAsync(int id)
    {
        await _db.Cars.Where(c => c.Id == id).ExecuteUpdateAsync(s => s.SetProperty(c => c.ViewCount, c => c.ViewCount + 1));
    }

    public async Task<List<string>> GetBrandsAsync() =>
        await _db.Cars.Where(c => c.IsActive).Select(c => c.Brand).Distinct().OrderBy(b => b).ToListAsync();

    private static string GenerateSlug(Car car) =>
        $"{car.Brand}-{car.Model}-{car.Year}-{car.Id}-{Guid.NewGuid().ToString()[..6]}"
        .ToLower().Replace(" ", "-");
}