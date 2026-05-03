using AutoSalon.Models;

namespace AutoSalon.Services;

public interface ILeadService
{
    Task<int> CreateAsync(Lead lead);
    Task<Lead?> GetByIdAsync(int id);
    Task<List<Lead>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<int> GetNewCountAsync();
    Task<int> GetFailedNotifyCountAsync();
    Task<List<Lead>> GetRecentAsync(int count);
    Task UpdateStatusAsync(int id, LeadStatus status);
    Task UpdateNoteAsync(int id, string? note);
    Task MarkNotifyFailedAsync(int id);
}