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
        var totalCars = await _cars.GetTotalCountAsync();
        var leads = await _leads.GetAllAsync();

        ViewBag.TotalCars = totalCars;
        ViewBag.TotalLeads = leads.Count;
        ViewBag.NewLeads = leads.Count(l => l.Status == AutoSalon.Models.LeadStatus.New);
        ViewBag.FailedNotifications = leads.Count(l => l.NotifyFailed);
        ViewBag.RecentLeads = leads.Take(10).ToList();

        return View();
    }
}