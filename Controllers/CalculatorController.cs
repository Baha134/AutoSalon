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
    [HttpGet]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal price,
        [FromQuery] int downPct = 20,
        [FromQuery] int months = 60)
    {
        if (price <= 0 || downPct < 0 || downPct > 90 || months < 6 || months > 120)
            return BadRequest(new { error = "Неверные параметры" });

        var s = await _settings.GetAsync();
        var rate = (double)s.CreditRate;

        var principal = price * (1 - downPct / 100m);
        var r = (decimal)rate / 100m / 12m;
        decimal monthly;

        if (r == 0)
        {
            monthly = Math.Round(principal / months);
        }
        else
        {
            var pow = (decimal)Math.Pow((double)(1 + r), months);
            monthly = Math.Round(principal * r * pow / (pow - 1));
        }

        var total = monthly * months;
        var overpay = total - principal;

        return Ok(new
        {
            monthly = (long)monthly,
            total = (long)total,
            overpay = (long)overpay,
            downAmount = (long)Math.Round(price * downPct / 100m),
            rate
        });
    }
}