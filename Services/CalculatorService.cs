namespace AutoSalon.Services;

public class CalculatorService : ICalculatorService
{
    public (decimal monthly, decimal total, decimal overpay) Calculate(decimal price, int downPct, int months)
    {
        var principal = price * (1 - downPct / 100m);
        var r = 18m / 100m / 12m;
        var pow = (decimal)Math.Pow((double)(1 + r), months);
        var monthly = Math.Round(principal * r * pow / (pow - 1));
        var total = monthly * months;
        return (monthly, total, Math.Round(total - principal));
    }
}