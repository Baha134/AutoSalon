namespace AutoSalon.Services;

public interface ICalculatorService
{
    (decimal monthly, decimal total, decimal overpay) Calculate(decimal price, int downPct, int months);
}

public class CalculatorService : ICalculatorService
{
    public (decimal monthly, decimal total, decimal overpay) Calculate(decimal price, int downPct, int months)
    {
        var principal = price * (1 - downPct / 100m);
        var r = 18m / 100m / 12m; // ставка 18% по умолчанию
        var monthly = principal * r * (decimal)Math.Pow((double)(1 + r), months)
                      / ((decimal)Math.Pow((double)(1 + r), months) - 1);
        var total = monthly * months;
        return (Math.Round(monthly), Math.Round(total), Math.Round(total - principal));
    }
}