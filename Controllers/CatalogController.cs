using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class CatalogController : Controller
{
    private readonly ICarService _cars;

    public CatalogController(ICarService cars) => _cars = cars;

    [HttpGet("/catalog")]
    public async Task<IActionResult> Index(CarFilterViewModel filter)
    {
        var (items, total) = await _cars.GetFilteredAsync(filter);
        var brands = await _cars.GetBrandsAsync();

        var vm = new CarListViewModel
        {
            Cars = items,
            Filter = filter,
            TotalCount = total,
            Brands = brands,
            BodyTypes = new List<string> { "Седан", "Кроссовер", "Внедорожник", "Хэтчбек", "Минивэн", "Купе", "Пикап" }
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_CatalogGrid", vm);

        return View(vm);
    }

    /// <summary>
    /// GET /catalog/search-json?q=toyota&limit=9
    /// Используется на странице сравнения для быстрого поиска авто.
    /// </summary>
    [HttpGet("/catalog/search-json")]
    public async Task<IActionResult> SearchJson(string q, int limit = 9)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(Array.Empty<object>());

        var cars = await _cars.SearchAsync(q.Trim(), limit);

        var result = cars.Select(c => new
        {
            id = c.Id,
            name = $"{c.Brand} {c.Model} {c.Year}",
            price = c.DisplayPrice,
            year = c.Year,
            mileage = c.DisplayMileage,
            photoPath = c.Photos.FirstOrDefault(p => p.IsMain)?.FilePath
                        ?? c.Photos.OrderBy(p => p.SortOrder).FirstOrDefault()?.FilePath
        });

        return Json(result);
    }
}