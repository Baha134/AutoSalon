using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class CarFilterViewModel
{
    public string? Brand { get; set; }
    public string? BodyType { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }

    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int? MileageMax { get; set; }

    // Ограничиваем длину поиска — защита от больших запросов
    [MaxLength(100)]
    public string? Search { get; set; }

    public string? Sort { get; set; } = "new";

    // null = только Active (по умолчанию в публичном каталоге)
    public AutoSalon.Models.CarStatus? Status { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    // true — если хоть один фильтр активен (нужно для Empty State)
    public bool HasActiveFilters =>
        !string.IsNullOrEmpty(Brand) ||
        !string.IsNullOrEmpty(BodyType) ||
        !string.IsNullOrEmpty(Transmission) ||
        !string.IsNullOrEmpty(FuelType) ||
        !string.IsNullOrEmpty(Search) ||
        PriceMin.HasValue || PriceMax.HasValue ||
        YearMin.HasValue || YearMax.HasValue ||
        MileageMax.HasValue;
}