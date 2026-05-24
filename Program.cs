using AutoSalon.Data;
using AutoSalon.Middleware;
using AutoSalon.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Лимит загрузки файлов — 100 МБ
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

// БД
// Читаем connection string: сначала переменная окружения, потом appsettings
var connectionString =
    Environment.GetEnvironmentVariable("AUTOSALON_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Connection string не задана. " +
        "Задай переменную окружения AUTOSALON_CONNECTION_STRING " +
        "или добавь ConnectionStrings:Default в appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// Локализация
builder.Services.AddLocalization();

// MVC
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddRazorPages(options =>
{
    // Закрываем страницу регистрации — только вход разрешён
    options.Conventions.AuthorizePage("/Account/Register");
});
// Антифоргери
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// Кеш
builder.Services.AddMemoryCache();

// HttpClient для Telegram
builder.Services.AddHttpClient();

// Сервисы приложения
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ICalculatorService, CalculatorService>();
builder.Services.AddScoped<IFavoriteService, CookieFavoriteService>();
builder.Services.AddScoped<INotifyService, TelegramNotifyService>();

// Session для Compare
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromDays(1);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("ru"),
        new CultureInfo("kk"),
        new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedData.InitAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();

// Статика из wwwroot (css, js, etc.)
app.UseStaticFiles();

// ✅ ЗАДАЧА 2.1: Фото отдаются из App_Data/uploads — папка вне wwwroot
var uploadsPhysicalPath = Path.Combine(app.Environment.ContentRootPath, "App_Data", "uploads");
Directory.CreateDirectory(uploadsPhysicalPath); // создаём при первом запуске
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPhysicalPath),
    RequestPath = "/uploads"
});

app.UseRequestLocalization();
app.UseRouting();
app.UseMiddleware<RateLimitMiddleware>();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();