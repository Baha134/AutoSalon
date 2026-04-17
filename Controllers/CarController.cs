using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class CarController : Controller
{
    private readonly ICarService _cars;
    private readonly ISettingsService _settings;

    public CarController(ICarService cars, ISettingsService settings)
    {
        _cars = cars;
        _settings = settings;
    }

    [HttpGet("car/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var car = await _cars.GetBySlugAsync(slug);
        if (car == null) return NotFound();

        // Удалённые slug — 410 Gone
        if (!car.IsActive)
        {
            Response.StatusCode = 410;
            return View("Gone");
        }

        await _cars.IncrementViewAsync(car.Id);
        var similar = await _cars.GetSimilarAsync(car.Id, car.Brand);
        var settings = await _settings.GetAsync();

        var vm = new CarDetailViewModel
        {
            Car = car,
            Settings = settings,
            Similar = similar
        };
        return View(vm);
    }
}