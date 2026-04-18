using AutoSalon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin")]
public class DashboardController : Controller
{
    private readonly ICarService _cars;
    private readonly ILeadService _leads;

    public DashboardController(ICarService cars, ILeadService leads)
    {
        _cars = cars;
        _leads = leads;
    }

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index()
    {
        // Три лёгких COUNT-запроса вместо загрузки всей таблицы
        var totalCars = await _cars.GetTotalCountAsync();
        var totalLeads = await _leads.GetCountAsync();
        var newLeads = await _leads.GetNewCountAsync();
        var failedCount = await _leads.GetFailedNotifyCountAsync();

        // Только 10 последних заявок
        var recentLeads = await _leads.GetRecentAsync(10);

        ViewBag.TotalCars = totalCars;
        ViewBag.TotalLeads = totalLeads;
        ViewBag.NewLeads = newLeads;
        ViewBag.FailedNotifications = failedCount;
        ViewBag.RecentLeads = recentLeads;

        return View("~/Areas/Admin/Views/Dashboard/Index.cshtml");
    }
}