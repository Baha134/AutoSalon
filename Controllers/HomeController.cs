using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}