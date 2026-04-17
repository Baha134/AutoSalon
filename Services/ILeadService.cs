using AutoSalon.Models;

namespace AutoSalon.Services;

public interface ILeadService
{
    Task<int> CreateAsync(Lead lead);
    Task<Lead?> GetByIdAsync(int id);
    Task<List<Lead>> GetAllAsync();
    Task UpdateStatusAsync(int id, LeadStatus status);
    Task MarkNotifyFailedAsync(int id);
}