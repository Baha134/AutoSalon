using AutoSalon.Models;

namespace AutoSalon.Services;

public interface ILeadService
{
    Task<int> CreateAsync(Lead lead);
    Task<List<Lead>> GetAllAsync();
    Task<Lead?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int id, LeadStatus status);
    Task DeleteAsync(int id);
}