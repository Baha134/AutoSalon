namespace AutoSalon.Models;

public enum LeadType { Call, WhatsApp, Credit }
public enum LeadStatus { New, InProgress, Done }

public class Lead
{
    public int Id { get; set; }
    public int? CarId { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public LeadType LeadType { get; set; }
    public string? Message { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string IP { get; set; } = "";
    public bool HoneypotTripped { get; set; }
    public bool NotifyFailed { get; set; }
    public Car? Car { get; set; }
}