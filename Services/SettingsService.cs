using AutoSalon.Data;
using AutoSalon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AutoSalon.Services;

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "salon_settings";

    public SettingsService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<SalonSettings> GetAsync()
    {
        if (_cache.TryGetValue(CacheKey, out SalonSettings? cached) && cached != null)
            return cached;

        var settings = await _db.SalonSettings.AsNoTracking().FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new SalonSettings
            {
                PhoneNumber = "+7 (000) 000-00-00",
                WhatsAppNumber = "70000000000",
                Address = "Город, улица, дом",
                WorkingHours = "Пн-Вс: 9:00 — 20:00",
                CreditRate = 18,
                TelegramBotToken = "",
                TelegramChatId = ""
            };
            _db.SalonSettings.Add(settings);
            await _db.SaveChangesAsync();
        }

        _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(5));
        return settings;
    }

    public async Task UpdateAsync(SalonSettings settings)
    {
        var existing = await _db.SalonSettings.FirstOrDefaultAsync();
        if (existing == null) return;

        existing.SalonName = settings.SalonName ?? "";
        existing.Phone = settings.Phone ?? "";
        existing.Email = settings.Email ?? "";
        existing.PhoneNumber = settings.PhoneNumber ?? "";
        existing.WhatsAppNumber = settings.WhatsAppNumber ?? "";
        existing.Address = settings.Address ?? "";
        existing.WorkingHours = settings.WorkingHours ?? "";
        existing.TelegramBotToken = settings.TelegramBotToken ?? "";
        existing.TelegramChatId = settings.TelegramChatId ?? "";
        existing.CreditRate = settings.CreditRate;

        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
    }
}