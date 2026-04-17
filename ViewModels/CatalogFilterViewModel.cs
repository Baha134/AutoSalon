namespace AutoSalon.ViewModels;

public class CarFilterViewModel
{
    public string? Brand { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}