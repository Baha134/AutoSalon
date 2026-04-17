namespace AutoSalon.Models;

public class SalonSettings
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string WhatsAppNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string WorkingHours { get; set; } = "";
    public string TelegramChatId { get; set; } = "";
    public string TelegramBotToken { get; set; } = "";
    public decimal CreditRate { get; set; } = 18;
}