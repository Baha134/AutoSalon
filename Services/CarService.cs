using AutoSalon.Data;
using AutoSalon.Models;
using AutoSalon.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Services;

public class CarService : ICarService
{
    private readonly AppDbContext _db;
    public CarService(AppDbContext db) => _db = db;

    public async Task<List<Car>> GetAllAsync(CatalogFilterViewModel? filter = null)
    {
        var q = _db.Cars.Include(c => c.Photos).Include(c => c.Badge)
                        .Where(c => c.IsActive).AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Brand))
                q = q.Where(c => c.Brand == filter.Brand);
            if (filter.MinPrice.HasValue)
                q = q.Where(c => c.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue)
                q = q.Where(c => c.Price <= filter.MaxPrice.Value);
            if (!string.IsNullOrEmpty(filter.BodyType))
                q = q.Where(c => c.BodyType == filter.BodyType);
            if (!string.IsNullOrEmpty(filter.Transmission))
                q = q.Where(c => c.Transmission == filter.Transmission);
        }

        return await q.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<Car?> GetBySlugAsync(string slug) =>
        await _db.Cars.Include(c => c.Photos).Include(c => c.Badge)
                      .FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<Car?> GetByIdAsync(int id) =>
        await _db.Cars.Include(c => c.Photos).Include(c => c.Badge)
                      .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<int> CreateAsync(Car car)
    {
        car.Slug = await GenerateSlugAsync(car);
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
        var car = await _db.Cars.FindAsync(id);
        if (car != null) { car.ViewCount++; await _db.SaveChangesAsync(); }
    }

    private async Task<string> GenerateSlugAsync(Car car)
    {
        var base_slug = $"{car.Brand}-{car.Model}-{car.Year}-{car.City}"
            .ToLower().Replace(" ", "-");
        var slug = base_slug;
        var i = 1;
        while (await _db.Cars.AnyAsync(c => c.Slug == slug))
            slug = $"{base_slug}-{i++}";
        return slug;
    }
}