using AutoSalon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoSalon.Data;

public static class SeedData
{
    public static async Task InitAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        // --- Роль Admin ---
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // --- Пользователь admin ---
        const string adminEmail = "admin@autosalon.kz";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, "Admin1234!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // --- Настройки салона ---
        if (!await db.SalonSettings.AnyAsync())
        {
            db.SalonSettings.Add(new SalonSettings
            {
                PhoneNumber = "+7 (777) 123-45-67",
                WhatsAppNumber = "77771234567",
                Address = "Кокшетау, ул. Абая 1",
                WorkingHours = "Пн-Вс: 9:00 — 20:00",
                CreditRate = 18,
                TelegramBotToken = "",
                TelegramChatId = ""
            });
            await db.SaveChangesAsync();
        }

        // --- Автомобили ---
        if (await db.Cars.AnyAsync()) return;

        var cars = new[]
        {
            new Car
            {
                Brand = "Toyota", Model = "Camry", Year = 2022,
                Price = 14_500_000, Mileage = 35_000,
                EngineVolume = 2.5m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Передний",
                FuelType = "Бензин", Color = "Белый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "toyota-camry-2022",
                Description = "Идеальное состояние, один владелец. ТО пройдено.",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Car
            {
                Brand = "Hyundai", Model = "Tucson", Year = 2021,
                Price = 12_900_000, Mileage = 58_000,
                EngineVolume = 2.0m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Серый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "hyundai-tucson-2021",
                Description = "Комплектация Prestige. Кожаный салон, панорамная крыша.",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Car
            {
                Brand = "Kia", Model = "K5", Year = 2023,
                Price = 16_200_000, Mileage = 12_000,
                EngineVolume = 2.5m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Передний",
                FuelType = "Бензин", Color = "Чёрный",
                City = "Астана", Status = CarStatus.Reserved, IsActive = true,
                Slug = "kia-k5-2023",
                Description = "Почти новый. На гарантии до 2026 года.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Car
            {
                Brand = "Lexus", Model = "RX 350", Year = 2020,
                Price = 22_000_000, Mileage = 75_000,
                EngineVolume = 3.5m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Синий",
                City = "Кокшетау", Status = CarStatus.Sold, IsActive = true,
                Slug = "lexus-rx350-2020",
                Description = "ПРОДАН. Показывается как пример.",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Car
            {
                Brand = "Volkswagen", Model = "Polo", Year = 2019,
                Price = 7_500_000, Mileage = 110_000,
                EngineVolume = 1.6m, BodyType = "Седан",
                Transmission = "Механика", DriveType = "Передний",
                FuelType = "Бензин", Color = "Красный",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "volkswagen-polo-2019",
                Description = "Экономичный городской автомобиль. Новая резина.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        db.Cars.AddRange(cars);
        await db.SaveChangesAsync();

        // --- Бейджи ---
        var savedCars = await db.Cars.ToListAsync();
        db.CarBadges.AddRange(
            new CarBadge { CarId = savedCars[0].Id, IsVerified = true, HasWarranty = true, WarrantyDays = 365 },
            new CarBadge { CarId = savedCars[1].Id, IsVerified = true, HasExchange = true },
            new CarBadge { CarId = savedCars[2].Id, IsVerified = true, HasWarranty = true, WarrantyDays = 730 },
            new CarBadge { CarId = savedCars[3].Id },
            new CarBadge { CarId = savedCars[4].Id, HasExchange = true }
        );
        await db.SaveChangesAsync();
    }
}