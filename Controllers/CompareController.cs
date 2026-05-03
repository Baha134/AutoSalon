using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AutoSalon.Controllers;

public class CompareController : Controller
{
    private const string SessionKey = "compare_ids";
    private const int MaxCars = 3;
    private readonly ICarService _cars;

    public CompareController(ICarService cars) => _cars = cars;

    private List<int> GetIds()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(json) ?? new();
    }

    private void SaveIds(List<int> ids) =>
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(ids));

    [HttpGet("/compare")]
    public async Task<IActionResult> Index()
    {
        var ids = GetIds();
        var carList = ids.Count > 0 ? await _cars.GetByIdsAsync(ids) : new();
        return View(new CompareViewModel { Cars = carList });
    }

    [HttpPost("/compare/add/{id:int}")]
    public IActionResult Add(int id)
    {
        var ids = GetIds();
        if (!ids.Contains(id))
        {
            if (ids.Count >= MaxCars) ids.RemoveAt(0);
            ids.Add(id);
            SaveIds(ids);
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { count = ids.Count });

        return Redirect(Request.Headers["Referer"].ToString() is { Length: > 0 } r ? r : "/");
    }

    [HttpPost("/compare/remove/{id:int}")]
    public IActionResult Remove(int id)
    {
        var ids = GetIds();
        ids.Remove(id);
        SaveIds(ids);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { count = ids.Count });

        return RedirectToAction("Index");
    }

    [HttpPost("/compare/clear")]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(SessionKey);
        return RedirectToAction("Index");
    }

    [HttpGet("/compare/count")]
    public IActionResult Count() => Ok(new { count = GetIds().Count });

    // БАГ ИСПРАВЛЕН: добавлен эндпоинт GET /compare/ids
    // Используется в Detail.cshtml для подсветки кнопки "Сравнить" при загрузке страницы
    [HttpGet("/compare/ids")]
    public IActionResult Ids() => Ok(GetIds());
}
