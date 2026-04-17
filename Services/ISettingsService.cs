using AutoSalon.Models;

namespace AutoSalon.Services;

public interface ISettingsService
{
    Task<SalonSettings> GetAsync();
    Task UpdateAsync(SalonSettings settings);
}