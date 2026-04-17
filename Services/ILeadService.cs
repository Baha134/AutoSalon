using AutoSalon.Models;

namespace AutoSalon.Services;

public interface ILeadService
{
    Task CreateAsync(Lead lead);
    Task<List<Lead>> GetAllAsync();
    Task UpdateStatusAsync(int id, LeadStatus status);
}