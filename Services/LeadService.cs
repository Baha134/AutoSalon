using AutoSalon.Data;
using AutoSalon.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Services;

public class LeadService : ILeadService
{
    private readonly AppDbContext _db;

    public LeadService(AppDbContext db) => _db = db;

    public async Task<int> CreateAsync(Lead lead)
    {
        _db.Leads.Add(lead);
        await _db.SaveChangesAsync();
        return lead.Id;
    }

    public async Task<List<Lead>> GetAllAsync() =>
        await _db.Leads
            .Include(l => l.Car)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<Lead?> GetByIdAsync(int id) =>
        await _db.Leads.Include(l => l.Car).FirstOrDefaultAsync(l => l.Id == id);

    public async Task UpdateStatusAsync(int id, LeadStatus status)
    {
        await _db.Leads
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.Status, status));
    }

    public async Task DeleteAsync(int id)
    {
        var lead = await _db.Leads.FindAsync(id);
        if (lead != null) { _db.Leads.Remove(lead); await _db.SaveChangesAsync(); }
    }
}