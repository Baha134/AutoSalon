using AutoSalon.Data;
using AutoSalon.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Services;

public class LeadService : ILeadService
{
    private readonly AppDbContext _db;
    public LeadService(AppDbContext db) => _db = db;

    public async Task CreateAsync(Lead lead)
    {
        _db.Leads.Add(lead);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Lead>> GetAllAsync() =>
        await _db.Leads.Include(l => l.Car)
                       .OrderByDescending(l => l.CreatedAt)
                       .ToListAsync();

    public async Task UpdateStatusAsync(int id, LeadStatus status)
    {
        var lead = await _db.Leads.FindAsync(id);
        if (lead != null) { lead.Status = status; await _db.SaveChangesAsync(); }
    }
}