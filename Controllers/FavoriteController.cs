using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class FavoriteController : Controller
{
    private readonly IFavoriteService _fav;
    private readonly ICarService _cars;

    public FavoriteController(IFavoriteService fav, ICarService cars)
    {
        _fav = fav;
        _cars = cars;
    }

    [HttpGet("/favorites")]
    public async Task<IActionResult> Index()
    {
        var ids = _fav.GetIds(HttpContext);
        var carList = ids.Count > 0 ? await _cars.GetByIdsAsync(ids) : new();
        return View(new FavoriteViewModel { Cars = carList });
    }

    [HttpPost("/favorites/toggle/{id:int}")]
    public IActionResult Toggle(int id)
    {
        bool isNow;

        if (_fav.Contains(HttpContext, id))
        {
            _fav.Remove(HttpContext, id);
            isNow = false;
        }
        else
        {
            _fav.Add(HttpContext, id);
            isNow = true;
        }

        // После Add/Remove счётчик уже актуален — просто читаем его
        var count = _fav.Count(HttpContext);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { count, isFavorite = isNow });

        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
    }

    [HttpGet("/favorites/count")]
    public IActionResult Count() =>
        Ok(new { count = _fav.Count(HttpContext) });
}