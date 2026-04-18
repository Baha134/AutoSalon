using AutoSalon.Models;
using AutoSalon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin/settings")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settings;

    public SettingsController(ISettingsService settings) => _settings = settings;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var s = await _settings.GetAsync();
        return View("~/Areas/Admin/Views/Settings/Index.cshtml", s);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SalonSettings model)
    {
        if (!ModelState.IsValid)
            return View("~/Areas/Admin/Views/Settings/Index.cshtml", model);
        await _settings.UpdateAsync(model);
        TempData["Success"] = "Настройки сохранены";
        return RedirectToAction("Index");
    }
}