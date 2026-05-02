using AutoSalon.Data;
using AutoSalon.Models;
using AutoSalon.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AutoSalon.Services;

public class CarService : ICarService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public CarService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<(List<Car> Items, int Total)> GetFilteredAsync(CarFilterViewModel filter)
    {
        var q = _db.Cars
            .Include(c => c.Photos)
            .Include(c => c.Badge)
            .Where(c => c.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Brand))
            q = q.Where(c => c.Brand == filter.Brand);

        if (!string.IsNullOrWhiteSpace(filter.BodyType))
            q = q.Where(c => c.BodyType == filter.BodyType);

        if (!string.IsNullOrWhiteSpace(filter.Transmission))
            q = q.Where(c => c.Transmission == filter.Transmission);

        if (!string.IsNullOrWhiteSpace(filter.FuelType))
            q = q.Where(c => c.FuelType == filter.FuelType);

        if (filter.PriceMin.HasValue)
            q = q.Where(c => c.Price >= filter.PriceMin.Value);

        if (filter.PriceMax.HasValue)
            q = q.Where(c => c.Price <= filter.PriceMax.Value);

        if (filter.YearMin.HasValue)
            q = q.Where(c => c.Year >= filter.YearMin.Value);

        if (filter.YearMax.HasValue)
            q = q.Where(c => c.Year <= filter.YearMax.Value);

        if (filter.MileageMax.HasValue)
            q = q.Where(c => c.Mileage <= filter.MileageMax.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search) && filter.Search.Length >= 2)
        {
            var term = filter.Search.Trim().ToLower();
            q = q.Where(c =>
                c.Brand.ToLower().Contains(term) ||
                c.Model.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)));
        }

        if (filter.Status.HasValue)
            q = q.Where(c => c.Status == filter.Status.Value);
        else
            q = q.Where(c => c.Status == CarStatus.Active);

        q = filter.Sort switch
        {
            "price_asc" => q.OrderBy(c => c.Price),
            "price_desc" => q.OrderByDescending(c => c.Price),
            "year_desc" => q.OrderByDescending(c => c.Year),
            "popular" => q.OrderByDescending(c => c.ViewCount),
            _ => q.OrderByDescending(c => c.CreatedAt)
        };

        var total = await q.CountAsync();
        var items = await q
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<string>> GetBrandsAsync()
    {
        const string cacheKey = "brands_list";
        if (_cache.TryGetValue(cacheKey, out List<string>? cached) && cached != null)
            return cached;

        var brands = await _db.Cars
            .Where(c => c.IsActive && c.Status == CarStatus.Active)
            .Select(c => c.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();

        _cache.Set(cacheKey, brands, TimeSpan.FromMinutes(5));
        return brands;
    }

    public async Task<Car?> GetBySlugAsync(string slug) =>
        await _db.Cars
            .Include(c => c.Photos.OrderBy(p => p.SortOrder))
            .Include(c => c.Badge)
            .FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<Car?> GetByIdAsync(int id) =>
        await _db.Cars
            .Include(c => c.Photos.OrderBy(p => p.SortOrder))
            .Include(c => c.Badge)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Car>> GetByIdsAsync(IEnumerable<int> ids) =>
        await _db.Cars
            .Include(c => c.Photos)
            .Include(c => c.Badge)
            .Where(c => ids.Contains(c.Id) && c.IsActive)
            .ToListAsync();

    public async Task<List<Car>> GetSimilarAsync(int carId, string brand, int count = 4) =>
        await _db.Cars
            .Include(c => c.Photos)
            .Where(c => c.Id != carId && c.Brand == brand && c.IsActive && c.Status == CarStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();

    /// <summary>
    /// Быстрый поиск по марке и модели — используется на странице сравнения.
    /// </summary>
    public async Task<List<Car>> SearchAsync(string query, int limit = 9)
    {
        var term = query.Trim().ToLower();

        return await _db.Cars
            .Include(c => c.Photos)
            .Where(c =>
                c.IsActive &&
                c.Status == CarStatus.Active &&
                (c.Brand.ToLower().Contains(term) ||
                 c.Model.ToLower().Contains(term) ||
                 (c.Brand + " " + c.Model).ToLower().Contains(term)))
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(Car car)
    {
        car.Slug = await GenerateSlugAsync(car);
        car.CreatedAt = DateTime.UtcNow;
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
        _cache.Remove("brands_list");
        return car.Id;
    }

    public async Task UpdateAsync(Car car)
    {
        _db.Cars.Update(car);
        await _db.SaveChangesAsync();
        _cache.Remove("brands_list");
    }

    public async Task DeleteAsync(int id)
    {
        var car = await _db.Cars.FindAsync(id);
        if (car != null)
        {
            car.IsActive = false;
            await _db.SaveChangesAsync();
            _cache.Remove("brands_list");
        }
    }

    public async Task IncrementViewAsync(int id)
    {
        await _db.Cars
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ViewCount, c => c.ViewCount + 1));
    }

    public async Task<int> GetTotalCountAsync() =>
        await _db.Cars.CountAsync(c => c.IsActive && c.Status == CarStatus.Active);

    private async Task<string> GenerateSlugAsync(Car car)
    {
        var baseSlug = $"{car.Brand}-{car.Model}-{car.Year}"
            .ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");

        var slug = baseSlug;
        var i = 1;
        while (await _db.Cars.AnyAsync(c => c.Slug == slug))
            slug = $"{baseSlug}-{i++}";

        return slug;
    }
}