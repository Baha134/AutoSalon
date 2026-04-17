namespace AutoSalon.Models;

public class CarPhoto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string FilePath { get; set; } = "";
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public Car? Car { get; set; }
}

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
    public Car? Car { get; set; }
}

public class SalonSettings
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string WhatsAppNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string WorkingHours { get; set; } = "";
    public string TelegramChatId { get; set; } = "";
    public decimal CreditRate { get; set; } = 18;
}

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public int? EntityId { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
}