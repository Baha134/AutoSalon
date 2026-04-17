namespace AutoSalon.Models;

public class CarBadge
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public bool IsVerified { get; set; }
    public bool HasWarranty { get; set; }
    public bool HasExchange { get; set; }
    public int? WarrantyDays { get; set; }
    public string? HistoryReportUrl { get; set; }
    public Car? Car { get; set; }
}