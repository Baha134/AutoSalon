namespace AutoSalon.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public int? EntityId { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
}