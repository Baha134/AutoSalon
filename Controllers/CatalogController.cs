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
}