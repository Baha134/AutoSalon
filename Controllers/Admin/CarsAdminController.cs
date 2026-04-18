using AutoSalon.Models;
using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin/cars")]
public class CarsAdminController : Controller
{
    private readonly ICarService _cars;
    private readonly IPhotoService _photos;

    public CarsAdminController(ICarService cars, IPhotoService photos)
    {
        _cars = cars;
        _photos = photos;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var filter = new CarFilterViewModel { PageSize = 50 };
        var (items, total) = await _cars.GetFilteredAsync(filter);
        return View("~/Areas/Admin/Views/CarsAdmin/Index.cshtml",
            new CarListViewModel { Cars = items, TotalCount = total, Filter = filter });
    }

    [HttpGet("create")]
    public IActionResult Create() =>
        View("~/Areas/Admin/Views/CarsAdmin/Create.cshtml", new AdminCarViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminCarViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("~/Areas/Admin/Views/CarsAdmin/Create.cshtml", vm);

        var car = MapToModel(vm);
        car.Slug = GenerateSlug(car.Brand, car.Model, car.Year);
        car.CreatedAt = DateTime.UtcNow;

        var id = await _cars.CreateAsync(car);

        if (vm.Photos.Count > 0)
            await _photos.SavePhotosAsync(vm.Photos, id);

        TempData["Success"] = "Автомобиль добавлен";
        return RedirectToAction("Index");
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var car = await _cars.GetByIdAsync(id);
        if (car == null) return NotFound();

        var vm = MapToViewModel(car);
        vm.ExistingPhotos = car.Photos.OrderBy(p => p.SortOrder).ToList();
        return View("~/Areas/Admin/Views/CarsAdmin/Edit.cshtml", vm);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminCarViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("~/Areas/Admin/Views/CarsAdmin/Edit.cshtml", vm);

        var car = await _cars.GetByIdAsync(id);
        if (car == null) return NotFound();

        ApplyToModel(car, vm);
        await _cars.UpdateAsync(car);

        if (vm.Photos.Count > 0)
            await _photos.SavePhotosAsync(vm.Photos, id);

        TempData["Success"] = "Изменения сохранены";
        return RedirectToAction("Index");
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _cars.DeleteAsync(id);
        TempData["Success"] = "Автомобиль удалён";
        return RedirectToAction("Index");
    }

    [HttpPost("{id:int}/photos/{photoId:int}/delete")]
    public async Task<IActionResult> DeletePhoto(int id, int photoId)
    {
        await _photos.DeletePhotoAsync(photoId);
        return Ok();
    }

    [HttpPost("{id:int}/photos/{photoId:int}/setmain")]
    public async Task<IActionResult> SetMain(int id, int photoId)
    {
        await _photos.SetMainPhotoAsync(photoId, id);
        return Ok();
    }

    [HttpPatch("{id:int}/photos/reorder")]
    public async Task<IActionResult> Reorder(int id, [FromBody] List<int> orderedIds)
    {
        await _photos.ReorderAsync(id, orderedIds);
        return Ok();
    }

    // --- helpers ---

    private static Car MapToModel(AdminCarViewModel vm) => new()
    {
        Brand = vm.Brand,
        Model = vm.Model,
        Year = vm.Year,
        Price = vm.Price,
        Mileage = vm.Mileage,
        EngineVolume = vm.EngineVolume,
        BodyType = vm.BodyType,
        Transmission = vm.Transmission,
        DriveType = vm.DriveType,
        FuelType = vm.FuelType,
        Color = vm.Color,
        City = vm.City,
        Description = vm.Description,
        Status = vm.Status,
        IsActive = vm.IsActive
    };

    private static void ApplyToModel(Car car, AdminCarViewModel vm)
    {
        car.Brand = vm.Brand; car.Model = vm.Model; car.Year = vm.Year;
        car.Price = vm.Price; car.Mileage = vm.Mileage; car.EngineVolume = vm.EngineVolume;
        car.BodyType = vm.BodyType; car.Transmission = vm.Transmission;
        car.DriveType = vm.DriveType; car.FuelType = vm.FuelType;
        car.Color = vm.Color; car.City = vm.City; car.Description = vm.Description;
        car.Status = vm.Status; car.IsActive = vm.IsActive;
    }

    private static AdminCarViewModel MapToViewModel(Car car) => new()
    {
        Id = car.Id,
        Brand = car.Brand,
        Model = car.Model,
        Year = car.Year,
        Price = car.Price,
        Mileage = car.Mileage,
        EngineVolume = car.EngineVolume,
        BodyType = car.BodyType,
        Transmission = car.Transmission,
        DriveType = car.DriveType,
        FuelType = car.FuelType,
        Color = car.Color,
        City = car.City,
        Description = car.Description,
        Status = car.Status,
        IsActive = car.IsActive,
        IsVerified = car.Badge?.IsVerified ?? false,
        HasWarranty = car.Badge?.HasWarranty ?? false,
        HasExchange = car.Badge?.HasExchange ?? false,
        WarrantyDays = car.Badge?.WarrantyDays
    };

    private static string GenerateSlug(string brand, string model, int year)
    {
        var raw = $"{brand}-{model}-{year}".ToLower()
            .Replace(" ", "-")
            .Replace("ё", "e")
            .Replace("ь", "")
            .Replace("ъ", "");
        return raw + "-" + Guid.NewGuid().ToString("N")[..6];
    }
}