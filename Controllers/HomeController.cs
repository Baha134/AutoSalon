using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class HomeController : Controller
{
    private readonly ICarService _cars;
    private readonly ISettingsService _settings;

    public HomeController(ICarService cars, ISettingsService settings)
    {
        _cars = cars;
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        var (items, total) = await _cars.GetFilteredAsync(new CarFilterViewModel { PageSize = 6, Sort = "new" });
        var brands = await _cars.GetBrandsAsync();
        var settings = await _settings.GetAsync();

        var vm = new HomeViewModel
        {
            FeaturedCars = items,
            TopBrands = brands,
            Settings = settings,
            TotalCars = total
        };
        return View(vm);
    }

    public async Task<IActionResult> About()
    {
        var settings = await _settings.GetAsync();
        return View(settings);
    }

    [Route("Home/Error/{code?}")]
    public IActionResult Error(int? code)
    {
        if (code == 404) return View("Error404");
        return View("Error");
    }
}