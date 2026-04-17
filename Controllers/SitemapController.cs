using AutoSalon.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class SitemapController : Controller
{
    private readonly ICarService _cars;

    public SitemapController(ICarService cars) => _cars = cars;

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var filter = new ViewModels.CarFilterViewModel { PageSize = 1000 };
        var (cars, _) = await _cars.GetFilteredAsync(filter);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        void AddUrl(string loc, string changefreq = "weekly", string priority = "0.5")
        {
            sb.AppendLine($"  <url>");
            sb.AppendLine($"    <loc>{loc}</loc>");
            sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{priority}</priority>");
            sb.AppendLine($"  </url>");
        }

        AddUrl(baseUrl + "/", "daily", "1.0");
        AddUrl(baseUrl + "/catalog", "daily", "0.9");
        AddUrl(baseUrl + "/about", "monthly", "0.4");

        foreach (var car in cars.Where(c => c.IsActive && c.Status != AutoSalon.Models.CarStatus.Sold))
        {
            AddUrl(baseUrl + $"/car/{car.Slug}", "weekly", "0.8");
        }

        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml");
    }
}