using AutoSalon.Data;
using AutoSalon.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Services;

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;

    public SettingsService(AppDbContext db) => _db = db;

    public async Task<SalonSettings> GetAsync()
    {
        var settings = await _db.SalonSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new SalonSettings
            {
                PhoneNumber = "+7 (000) 000-00-00",
                WhatsAppNumber = "70000000000",
                Address = "Город, улица, дом",
                WorkingHours = "Пн-Вс: 9:00 — 20:00",
                CreditRate = 18
            };
            _db.SalonSettings.Add(settings);
            await _db.SaveChangesAsync();
        }
        return settings;
    }

    public async Task UpdateAsync(SalonSettings settings)
    {
        _db.SalonSettings.Update(settings);
        await _db.SaveChangesAsync();
    }
}