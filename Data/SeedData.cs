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
            },
            new Car
            {
                Brand = "BMW", Model = "5 Series", Year = 2021,
                Price = 28_500_000, Mileage = 42_000,
                EngineVolume = 2.0m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Задний",
                FuelType = "Бензин", Color = "Чёрный",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "bmw-5-series-2021",
                Description = "Комплектация M Sport. Подогрев сидений, адаптивный круиз.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Car
            {
                Brand = "Mercedes-Benz", Model = "E 200", Year = 2020,
                Price = 26_000_000, Mileage = 61_000,
                EngineVolume = 2.0m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Задний",
                FuelType = "Бензин", Color = "Серебристый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "mercedes-e200-2020",
                Description = "Полная комплектация. Панорамный люк, вентиляция сидений.",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Car
            {
                Brand = "Audi", Model = "A6", Year = 2021,
                Price = 27_000_000, Mileage = 38_000,
                EngineVolume = 2.0m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Белый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "audi-a6-2021",
                Description = "Quattro. Виртуальная панель приборов, матричный свет.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new Car
            {
                Brand = "Toyota", Model = "RAV4", Year = 2022,
                Price = 18_700_000, Mileage = 28_000,
                EngineVolume = 2.5m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Гибрид", Color = "Зелёный",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "toyota-rav4-hybrid-2022",
                Description = "Гибрид. Расход 5.8 л/100км. Идеален для города и трассы.",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Car
            {
                Brand = "Kia", Model = "Sportage", Year = 2023,
                Price = 15_400_000, Mileage = 9_000,
                EngineVolume = 2.0m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Белый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "kia-sportage-2023",
                Description = "Новое поколение. На гарантии. Панорамная крыша.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Car
            {
                Brand = "Hyundai", Model = "Elantra", Year = 2022,
                Price = 10_200_000, Mileage = 44_000,
                EngineVolume = 1.6m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Передний",
                FuelType = "Бензин", Color = "Синий",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "hyundai-elantra-2022",
                Description = "Новое поколение. Отличный расход топлива.",
                CreatedAt = DateTime.UtcNow.AddDays(-9)
            },
            new Car
            {
                Brand = "Chevrolet", Model = "Equinox", Year = 2020,
                Price = 11_800_000, Mileage = 72_000,
                EngineVolume = 1.5m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Передний",
                FuelType = "Бензин", Color = "Чёрный",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "chevrolet-equinox-2020",
                Description = "Полная комплектация Premier. Кожаный салон.",
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new Car
            {
                Brand = "Nissan", Model = "X-Trail", Year = 2021,
                Price = 13_600_000, Mileage = 51_000,
                EngineVolume = 2.5m, BodyType = "Кроссовер",
                Transmission = "Вариатор", DriveType = "Полный",
                FuelType = "Бензин", Color = "Коричневый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "nissan-xtrail-2021",
                Description = "7 мест. Третий ряд сидений. Хорошее состояние.",
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new Car
            {
                Brand = "Mazda", Model = "CX-5", Year = 2022,
                Price = 17_300_000, Mileage = 22_000,
                EngineVolume = 2.5m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Красный",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "mazda-cx5-2022",
                Description = "Soul Red Crystal. Максимальная комплектация Signature.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Car
            {
                Brand = "Subaru", Model = "Forester", Year = 2021,
                Price = 16_800_000, Mileage = 33_000,
                EngineVolume = 2.5m, BodyType = "Кроссовер",
                Transmission = "Вариатор", DriveType = "Полный",
                FuelType = "Бензин", Color = "Серый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "subaru-forester-2021",
                Description = "EyeSight. Симметричный полный привод. Отличный выбор для бездорожья.",
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            },
            new Car
            {
                Brand = "Honda", Model = "CR-V", Year = 2020,
                Price = 14_100_000, Mileage = 66_000,
                EngineVolume = 1.5m, BodyType = "Кроссовер",
                Transmission = "Вариатор", DriveType = "Полный",
                FuelType = "Бензин", Color = "Белый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "honda-crv-2020",
                Description = "Турбо. Комплектация Touring. Подогрев руля и сидений.",
                CreatedAt = DateTime.UtcNow.AddDays(-16)
            },
            new Car
            {
                Brand = "Toyota", Model = "Land Cruiser 200", Year = 2019,
                Price = 38_000_000, Mileage = 88_000,
                EngineVolume = 4.5m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Дизель", Color = "Чёрный",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "toyota-lc200-2019",
                Description = "Executive. Полный фарш. Пробег подтверждён дилером.",
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new Car
            {
                Brand = "Mitsubishi", Model = "Outlander", Year = 2021,
                Price = 13_200_000, Mileage = 47_000,
                EngineVolume = 2.4m, BodyType = "Кроссовер",
                Transmission = "Вариатор", DriveType = "Полный",
                FuelType = "Бензин", Color = "Белый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "mitsubishi-outlander-2021",
                Description = "7 мест. S-AWC. Хорошее состояние, один владелец.",
                CreatedAt = DateTime.UtcNow.AddDays(-13)
            },
            new Car
            {
                Brand = "Skoda", Model = "Octavia", Year = 2020,
                Price = 9_800_000, Mileage = 81_000,
                EngineVolume = 1.4m, BodyType = "Лифтбек",
                Transmission = "Робот", DriveType = "Передний",
                FuelType = "Бензин", Color = "Серебристый",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "skoda-octavia-2020",
                Description = "Style. Виртуальная панель, адаптивный круиз, CarPlay.",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Car
            {
                Brand = "Renault", Model = "Duster", Year = 2021,
                Price = 8_400_000, Mileage = 54_000,
                EngineVolume = 1.3m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Передний",
                FuelType = "Бензин", Color = "Оранжевый",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "renault-duster-2021",
                Description = "Обновлённый Duster. Турбо. Хорошая проходимость.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new Car
            {
                Brand = "Geely", Model = "Atlas Pro", Year = 2022,
                Price = 9_600_000, Mileage = 31_000,
                EngineVolume = 1.5m, BodyType = "Кроссовер",
                Transmission = "Робот", DriveType = "Передний",
                FuelType = "Бензин", Color = "Белый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "geely-atlas-pro-2022",
                Description = "Богатая комплектация. 360° камера, беспроводная зарядка.",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Car
            {
                Brand = "Haval", Model = "Jolion", Year = 2023,
                Price = 10_500_000, Mileage = 14_000,
                EngineVolume = 1.5m, BodyType = "Кроссовер",
                Transmission = "Робот", DriveType = "Передний",
                FuelType = "Бензин", Color = "Чёрный",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "haval-jolion-2023",
                Description = "На гарантии. Как новый. Полный фарш.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Car
            {
                Brand = "Chery", Model = "Tiggo 7 Pro", Year = 2022,
                Price = 9_200_000, Mileage = 27_000,
                EngineVolume = 1.5m, BodyType = "Кроссовер",
                Transmission = "Робот", DriveType = "Передний",
                FuelType = "Бензин", Color = "Синий",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "chery-tiggo7pro-2022",
                Description = "Люксовая комплектация. Панорамный люк, 360° камера.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Car
            {
                Brand = "Ford", Model = "Explorer", Year = 2020,
                Price = 19_500_000, Mileage = 68_000,
                EngineVolume = 3.0m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Серый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "ford-explorer-2020",
                Description = "ST-Line. 7 мест. Мощный и комфортный семейный внедорожник.",
                CreatedAt = DateTime.UtcNow.AddDays(-17)
            },
            new Car
            {
                Brand = "Porsche", Model = "Cayenne", Year = 2021,
                Price = 55_000_000, Mileage = 29_000,
                EngineVolume = 3.0m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Гибрид", Color = "Белый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "porsche-cayenne-e-hybrid-2021",
                Description = "E-Hybrid. 462 л.с. Спортивный пакет. Панорамная крыша.",
                CreatedAt = DateTime.UtcNow.AddDays(-9)
            },
            new Car
            {
                Brand = "Volvo", Model = "XC60", Year = 2021,
                Price = 32_000_000, Mileage = 41_000,
                EngineVolume = 2.0m, BodyType = "Кроссовер",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Гибрид", Color = "Синий",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "volvo-xc60-recharge-2021",
                Description = "Recharge. Plug-in гибрид. Inscription. Bowers & Wilkins аудио.",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Car
            {
                Brand = "Land Rover", Model = "Defender 110", Year = 2022,
                Price = 62_000_000, Mileage = 18_000,
                EngineVolume = 3.0m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Дизель", Color = "Зелёный",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "landrover-defender110-2022",
                Description = "X-Dynamic SE. Air Suspension. Meridian аудио. Пневмоподвеска.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Car
            {
                Brand = "Tesla", Model = "Model 3", Year = 2022,
                Price = 24_000_000, Mileage = 19_000,
                EngineVolume = 0m, BodyType = "Седан",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Электро", Color = "Белый",
                City = "Алматы", Status = CarStatus.Active, IsActive = true,
                Slug = "tesla-model3-awd-2022",
                Description = "Long Range AWD. Запас хода 580 км. Автопилот включён.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new Car
            {
                Brand = "Kia", Model = "Sorento", Year = 2022,
                Price = 19_800_000, Mileage = 36_000,
                EngineVolume = 2.5m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Гибрид", Color = "Чёрный",
                City = "Кокшетау", Status = CarStatus.Active, IsActive = true,
                Slug = "kia-sorento-hybrid-2022",
                Description = "7 мест. Гибрид. Максимальная комплектация Signature.",
                CreatedAt = DateTime.UtcNow.AddDays(-11)
            },
            new Car
            {
                Brand = "Genesis", Model = "GV80", Year = 2021,
                Price = 42_000_000, Mileage = 33_000,
                EngineVolume = 3.5m, BodyType = "Внедорожник",
                Transmission = "Автомат", DriveType = "Полный",
                FuelType = "Бензин", Color = "Серебристый",
                City = "Астана", Status = CarStatus.Active, IsActive = true,
                Slug = "genesis-gv80-2021",
                Description = "Люксовый корейский внедорожник. Массаж, вентиляция, HUD.",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
        };

        db.Cars.AddRange(cars);
        await db.SaveChangesAsync();

        // --- Бейджи ---
        var savedCars = await db.Cars.ToListAsync();
        var badges = savedCars.Select((car, i) => new CarBadge
        {
            CarId = car.Id,
            IsVerified = i % 2 == 0,
            HasWarranty = i % 3 == 0,
            WarrantyDays = i % 3 == 0 ? 365 : 0,
            HasExchange = i % 4 == 0
        }).ToList();

        db.CarBadges.AddRange(badges);
        await db.SaveChangesAsync();
    }
}