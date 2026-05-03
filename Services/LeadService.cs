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
        lead.CreatedAt = DateTime.UtcNow;
        _db.Leads.Add(lead);
        await _db.SaveChangesAsync();
        return lead.Id;
    }

    public async Task<Lead?> GetByIdAsync(int id) =>
        await _db.Leads
            .Include(l => l.Car)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<Lead>> GetAllAsync() =>
        await _db.Leads
            .Include(l => l.Car)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<int> GetCountAsync() =>
        await _db.Leads.CountAsync();

    public async Task<int> GetNewCountAsync() =>
        await _db.Leads.CountAsync(l => l.Status == LeadStatus.New);

    public async Task<int> GetFailedNotifyCountAsync() =>
        await _db.Leads.CountAsync(l => l.NotifyFailed);

    public async Task<List<Lead>> GetRecentAsync(int count) =>
        await _db.Leads
            .Include(l => l.Car)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task UpdateStatusAsync(int id, LeadStatus status)
    {
        var lead = await _db.Leads.FindAsync(id);
        if (lead != null)
        {
            lead.Status = status;
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateNoteAsync(int id, string? note)
    {
        var lead = await _db.Leads.FindAsync(id);
        if (lead != null)
        {
            lead.AdminNote = note;
            await _db.SaveChangesAsync();
        }
    }

    public async Task MarkNotifyFailedAsync(int id)
    {
        var lead = await _db.Leads.FindAsync(id);
        if (lead != null)
        {
            lead.NotifyFailed = true;
            await _db.SaveChangesAsync();
        }
    }
}