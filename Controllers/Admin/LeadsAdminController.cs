using AutoSalon.Models;
using AutoSalon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Area("Admin")]
[Route("admin/leads")]
public class LeadsAdminController : Controller
{
    private readonly ILeadService _leads;

    public LeadsAdminController(ILeadService leads) => _leads = leads;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var items = await _leads.GetAllAsync();
        return View("~/Areas/Admin/Views/Leads/Index.cshtml", items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var lead = await _leads.GetByIdAsync(id);
        if (lead == null) return NotFound();
        return View("~/Areas/Admin/Views/Leads/Detail.cshtml", lead);
    }

    [HttpPost("{id:int}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, LeadStatus status)
    {
        await _leads.UpdateStatusAsync(id, status);
        TempData["Success"] = "Статус обновлён";
        return RedirectToAction("Detail", new { id });
    }
}