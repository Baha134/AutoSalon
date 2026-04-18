namespace AutoSalon.Services;

public interface ICalculatorService
{
    (decimal monthly, decimal total, decimal overpay) Calculate(decimal price, int downPct, int months, decimal rate);
}