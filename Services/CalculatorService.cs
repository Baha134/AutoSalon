namespace AutoSalon.Services;

public class CalculatorService : ICalculatorService
{
    public (decimal monthly, decimal total, decimal overpay) Calculate(
        decimal price, int downPct, int months, decimal rate)
    {
        var principal = price * (1 - downPct / 100m);
        var r = rate / 100m / 12m;

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
        return (monthly, total, Math.Round(total - principal));
    }
}