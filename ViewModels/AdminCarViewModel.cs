using AutoSalon.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class AdminCarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Укажите марку")]
    [MaxLength(50, ErrorMessage = "Не более 50 символов")]
    public string Brand { get; set; } = "";

    [Required(ErrorMessage = "Укажите модель")]
    [MaxLength(100, ErrorMessage = "Не более 100 символов")]
    public string Model { get; set; } = "";

    [Required(ErrorMessage = "Укажите год")]
    [Range(1990, 2100, ErrorMessage = "Год должен быть от 1990 до 2100")]
    public int Year { get; set; } = DateTime.Now.Year;

    [Required(ErrorMessage = "Укажите цену")]
    [Range(0, 999_999_999, ErrorMessage = "Цена должна быть больше 0")]
    public decimal Price { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Некорректный пробег")]
    public int Mileage { get; set; }

    [Range(0, 20, ErrorMessage = "Некорректный объём двигателя")]
    public decimal EngineVolume { get; set; }

    [MaxLength(50)]
    public string BodyType { get; set; } = "";

    [MaxLength(50)]
    public string Transmission { get; set; } = "";

    [MaxLength(50)]
    public string DriveType { get; set; } = "";

    [MaxLength(50)]
    public string FuelType { get; set; } = "";

    [MaxLength(50)]
    public string Color { get; set; } = "";

    [MaxLength(100)]
    public string City { get; set; } = "";

    [MaxLength(2000)]
    public string? Description { get; set; }

    public CarStatus Status { get; set; } = CarStatus.Active;
    public bool IsActive { get; set; } = true;

    // Badge
    public bool IsVerified { get; set; }
    public bool HasWarranty { get; set; }
    public bool HasExchange { get; set; }

    [Range(1, 3650, ErrorMessage = "Дней гарантии: от 1 до 3650")]
    public int? WarrantyDays { get; set; }

    // Photos
    public List<IFormFile> Photos { get; set; } = new();
    public List<CarPhoto> ExistingPhotos { get; set; } = new();
}