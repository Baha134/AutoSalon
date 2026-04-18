using AutoSalon.Models;
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

        // Slug не существует вообще
        if (car == null)
            return NotFound();

        // Мягко удалённые авто — 410 Gone (slug больше не используется)
        if (!car.IsActive)
        {
            Response.StatusCode = 410;
            ViewData["NoIndex"] = true;
            return View("Gone");
        }

        // Проданные авто — показываем страницу, но запрещаем индексацию
        if (car.Status == CarStatus.Sold)
        {
            ViewData["NoIndex"] = true;
        }

        await _cars.IncrementViewAsync(car.Id);

        var similar = await _cars.GetSimilarAsync(car.Id, car.Brand);
        var settings = await _settings.GetAsync();

        // Open Graph для превью в WhatsApp / Telegram
        var mainPhoto = car.Photos.FirstOrDefault(p => p.IsMain) ?? car.Photos.FirstOrDefault();
        if (mainPhoto != null)
        {
            ViewData["OgTitle"] = $"{car.Brand} {car.Model} {car.Year} — {car.DisplayPrice}";
            ViewData["OgDescription"] = car.Description ?? $"{car.Brand} {car.Model} {car.Year}, {car.Mileage:N0} км";
            ViewData["OgImage"] = $"{Request.Scheme}://{Request.Host}{mainPhoto.FilePath}";
        }

        var vm = new CarDetailViewModel
        {
            Car = car,
            Settings = settings,
            Similar = similar
        };
        return View(vm);
    }
}