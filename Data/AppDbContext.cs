using AutoSalon.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarPhoto> CarPhotos => Set<CarPhoto>();
    public DbSet<CarBadge> CarBadges => Set<CarBadge>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<SalonSettings> SalonSettings => Set<SalonSettings>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Car>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);

        builder.Entity<Car>()
            .Property(c => c.EngineVolume)
            .HasPrecision(4, 1);

        builder.Entity<SalonSettings>()
            .Property(s => s.CreditRate)
            .HasPrecision(5, 2);

        // Индексы для таблицы Cars
        builder.Entity<Car>()
            .HasIndex(c => new { c.IsActive, c.Status })
            .HasDatabaseName("IX_Cars_IsActive_Status");

        builder.Entity<Car>()
            .HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Cars_CreatedAt");

        builder.Entity<Car>()
            .HasIndex(c => c.Brand)
            .HasDatabaseName("IX_Cars_Brand");

        builder.Entity<Car>()
            .HasIndex(c => c.Price)
            .HasDatabaseName("IX_Cars_Price");

        builder.Entity<Car>()
            .HasIndex(c => c.Year)
            .HasDatabaseName("IX_Cars_Year");

        builder.Entity<Car>()
            .HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Cars_Slug_Unique");
    }
}