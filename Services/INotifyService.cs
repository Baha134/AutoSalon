using AutoSalon.Models;

namespace AutoSalon.Services;

public enum NotifyResult
{
    Sent,           // успешно отправлено
    NotConfigured,  // Telegram не настроен — не ошибка
    Failed          // реальная ошибка отправки
}

public interface INotifyService
{
    Task<NotifyResult> SendLeadAsync(Lead lead);
}