using AutoSalon.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class AdminCarViewModel
{
    public int Id { get; set; }

    [Required] public string Brand { get; set; } = "";
    [Required] public string Model { get; set; } = "";
    [Required] public int Year { get; set; } = DateTime.Now.Year;
    [Required] public decimal Price { get; set; }
    public int Mileage { get; set; }
    public decimal EngineVolume { get; set; }
    public string BodyType { get; set; } = "";
    public string Transmission { get; set; } = "";
    public string DriveType { get; set; } = "";
    public string FuelType { get; set; } = "";
    public string Color { get; set; } = "";
    public string City { get; set; } = "";
    public string? Description { get; set; }
    public CarStatus Status { get; set; } = CarStatus.Active;
    public bool IsActive { get; set; } = true;

    // Бейджи
    public bool IsVerified { get; set; }
    public bool HasWarranty { get; set; }
    public bool HasExchange { get; set; }
    public int? WarrantyDays { get; set; }

    // Фото (загрузка)
    public List<IFormFile> Photos { get; set; } = new();
    public List<CarPhoto> ExistingPhotos { get; set; } = new();
}