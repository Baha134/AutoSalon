namespace AutoSalon.Models;

public enum CarStatus { Active, Sold, Reserved }

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public decimal EngineVolume { get; set; }
    public string BodyType { get; set; } = "";
    public string Transmission { get; set; } = "";
    public string DriveType { get; set; } = "";
    public string FuelType { get; set; } = "";
    public string Color { get; set; } = "";
    public string City { get; set; } = "";
    public string? Description { get; set; }
    public string Slug { get; set; } = "";
    public CarStatus Status { get; set; } = CarStatus.Active;
    public int ViewCount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public List<CarPhoto> Photos { get; set; } = new();
    public CarBadge? Badge { get; set; }
    public List<Lead> Leads { get; set; } = new();

    // Вспомогательные свойства (не в БД)
    public CarPhoto? MainPhoto => Photos.FirstOrDefault(p => p.IsMain) ?? Photos.FirstOrDefault();
    public string DisplayPrice => Price.ToString("N0") + " ₸";
    public string DisplayMileage => Mileage == 0 ? "Новый" : Mileage.ToString("N0") + " км";
}