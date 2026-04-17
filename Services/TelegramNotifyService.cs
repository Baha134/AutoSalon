using AutoSalon.Models;
using AutoSalon.Services;

namespace AutoSalon.Services;

public class TelegramNotifyService : INotifyService
{
    private readonly ISettingsService _settings;
    private readonly ILogger<TelegramNotifyService> _logger;
    private readonly HttpClient _http;

    public TelegramNotifyService(
        ISettingsService settings,
        ILogger<TelegramNotifyService> logger,
        IHttpClientFactory httpFactory)
    {
        _settings = settings;
        _logger = logger;
        _http = httpFactory.CreateClient();
    }

    public async Task<bool> SendLeadAsync(Lead lead)
    {
        try
        {
            var s = await _settings.GetAsync();

            if (string.IsNullOrEmpty(s.TelegramBotToken) || string.IsNullOrEmpty(s.TelegramChatId))
            {
                _logger.LogWarning("Telegram не настроен (токен или ChatId пустые)");
                return false;
            }

            var leadTypeLabel = lead.LeadType switch
            {
                LeadType.Call => "📞 Звонок",
                LeadType.WhatsApp => "💬 WhatsApp",
                LeadType.Credit => "💳 Кредит",
                _ => "Заявка"
            };

            var carInfo = lead.Car != null
                ? $"🚗 Авто: {lead.Car.Brand} {lead.Car.Model} {lead.Car.Year} — {lead.Car.DisplayPrice}\n"
                : "";

            var text = $"""
                🔔 Новая заявка [{leadTypeLabel}]

                👤 Имя: {lead.Name}
                📱 Телефон: {lead.Phone}
                {carInfo}{(string.IsNullOrEmpty(lead.Message) ? "" : $"💬 Сообщение: {lead.Message}\n")}
                🕐 {lead.CreatedAt:dd.MM.yyyy HH:mm}
                """;

            var url = $"https://api.telegram.org/bot{s.TelegramBotToken}/sendMessage";
            var payload = new
            {
                chat_id = s.TelegramChatId,
                text = text,
                parse_mode = "HTML"
            };

            var response = await _http.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки уведомления в Telegram");
            return false;
        }
    }
}