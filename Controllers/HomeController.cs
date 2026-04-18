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

    [HttpGet("/")]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var (items, total) = await _cars.GetFilteredAsync(
            new CarFilterViewModel { PageSize = 6, Sort = "new" });
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

    // Явный роут /about — теперь ссылка в navbar работает
    [HttpGet("/about")]
    public async Task<IActionResult> About()
    {
        var settings = await _settings.GetAsync();
        return View(settings);
    }

    // Кастомные страницы ошибок — вызываются через UseStatusCodePagesWithReExecute
    [Route("Home/Error/{code?}")]
    public IActionResult Error(int? code)
    {
        return code switch
        {
            404 => View("Error404"),
            410 => View("Error410"),
            _ => View("Error500")
        };
    }
}