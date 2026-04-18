using AutoSalon.Data;
using AutoSalon.Middleware;
using AutoSalon.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- БД ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// --- Identity ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// --- MVC ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --- Кеш (используется в CarService и SettingsService) ---
builder.Services.AddMemoryCache();

// --- HttpClient для Telegram ---
builder.Services.AddHttpClient();

// --- Сервисы приложения ---
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ICalculatorService, CalculatorService>();
builder.Services.AddScoped<IFavoriteService, CookieFavoriteService>();
builder.Services.AddScoped<INotifyService, TelegramNotifyService>();

// --- Session для Compare ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromDays(1);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

var app = builder.Build();

// --- Seed ---
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitAsync(scope.ServiceProvider);
}

// --- Middleware pipeline (порядок важен!) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error/500");
    app.UseHsts();
}

// Кастомные страницы ошибок (404, 500 и т.д.)
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Rate limit ПЕРЕД авторизацией — защищаем до аутентификации
app.UseMiddleware<RateLimitMiddleware>();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// --- Маршруты ---
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();