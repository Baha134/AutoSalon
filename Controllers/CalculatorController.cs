using AutoSalon.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

[Route("api/calculator")]
[ApiController]
public class CalculatorController : ControllerBase
{
    private readonly ICalculatorService _calc;
    private readonly ISettingsService _settings;

    public CalculatorController(ICalculatorService calc, ISettingsService settings)
    {
        _calc = calc;
        _settings = settings;
    }

    /// <summary>
    /// GET /api/calculator?price=10000000&amp;downPct=20&amp;months=60
    /// Возвращает ежемесячный платёж, итого и переплату.
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal price,
        [FromQuery] int downPct = 20,
        [FromQuery] int months = 60)
    {
        if (price <= 0 || downPct < 0 || downPct > 90 || months < 6 || months > 120)
            return BadRequest(new { error = "Неверные параметры" });

        var s = await _settings.GetAsync();
        var rate = s.CreditRate;

        var (monthly, total, overpay) = _calc.Calculate(price, downPct, months, rate);
        var downAmount = Math.Round(price * downPct / 100m);

        return Ok(new
        {
            monthly = (long)monthly,
            total = (long)total,
            overpay = (long)overpay,
            downAmount = (long)downAmount,
            rate = (double)rate
        });
    }

    /// <summary>
    /// GET /api/calculator/schedule?price=10000000&amp;downPct=20&amp;months=60&amp;rate=18
    /// Возвращает полный помесячный график платежей.
    /// Если rate не передан — берётся из настроек салона.
    /// </summary>
    [HttpGet("schedule")]
    public async Task<IActionResult> Schedule(
        [FromQuery] decimal price,
        [FromQuery] int downPct = 20,
        [FromQuery] int months = 60,
        [FromQuery] decimal rate = 0,
        [FromQuery] decimal insurance = 0)
    {
        if (price <= 0 || downPct < 0 || downPct > 90 || months < 6 || months > 120)
            return BadRequest(new { error = "Неверные параметры" });

        if (rate <= 0)
        {
            var s = await _settings.GetAsync();
            rate = s.CreditRate;
        }

        var down = Math.Round(price * downPct / 100m);
        var principal = price - down + insurance;
        var r = rate / 100m / 12m;

        decimal monthly;
        if (r == 0)
            monthly = Math.Round(principal / months);
        else
        {
            var pow = (decimal)Math.Pow((double)(1 + r), months);
            monthly = Math.Round(principal * r * pow / (pow - 1));
        }

        var rows = new List<object>();
        var balance = principal;
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);

        for (int i = 0; i < months; i++)
        {
            var intPart = Math.Round(balance * r, 2);
            var prinPart = Math.Round(monthly - intPart, 2);
            balance = Math.Max(0, balance - prinPart);

            rows.Add(new
            {
                date = startDate.AddMonths(i).ToString("dd.MM.yyyy"),
                payment = (long)monthly,
                principal = (long)Math.Round(prinPart),
                interest = (long)Math.Round(intPart),
                balance = (long)Math.Round(balance)
            });
        }

        var total = monthly * months;
        var overpay = total - principal;

        return Ok(new
        {
            monthly = (long)monthly,
            total = (long)total,
            overpay = (long)overpay,
            down = (long)down,
            principal = (long)principal,
            rate = (double)rate,
            months,
            rows
        });
    }
}


/// <summary>
/// Отдельный MVC-контроллер для страницы /calculator
/// </summary>
[Route("calculator")]
public class CalculatorPageController : Controller
{
    private readonly ISettingsService _settings;

    public CalculatorPageController(ISettingsService settings)
    {
        _settings = settings;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var settings = await _settings.GetAsync();
        ViewData["Title"] = "Кредитный калькулятор";
        ViewData["Description"] = "Рассчитайте ежемесячный платёж по автокредиту онлайн. Полный график погашения с разбивкой на основной долг и проценты.";
        return View("~/Views/Calculator/Index.cshtml", settings);
    }
}