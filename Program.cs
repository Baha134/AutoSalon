using AutoSalon.Data;
using AutoSalon.Middleware;
using AutoSalon.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

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

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

// ===== ЛОКАЛИЗАЦИЯ =====
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

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
// =======================

var app = builder.Build();

// Seed
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
app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseMiddleware<RateLimitMiddleware>();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Маршруты
app.MapControllerRoute(
    name: "language",
    pattern: "language/set",
    defaults: new { controller = "Language", action = "Set" });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();