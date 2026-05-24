using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

/// <summary>
/// Публичная регистрация закрыта (задача 2.3).
/// Вход только через /Identity/Account/Login (стандартный Identity UI).
/// </summary>
public class AccountController : Controller
{
    // GET /account/register — закрыто
    [HttpGet]
    public IActionResult Register() => NotFound();

    // POST /account/register — закрыто
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RegisterPost() => NotFound();
}