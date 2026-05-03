using AutoSalon.Models;
using AutoSalon.Services;
using AutoSalon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers;

public class LeadController : Controller
{
    private readonly ILeadService _leads;
    private readonly INotifyService _notify;

    public LeadController(ILeadService leads, INotifyService notify)
    {
        _leads = leads;
        _notify = notify;
    }

    [HttpPost("/leads")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeadFormViewModel vm)
    {
        // Honeypot — бот заполнил скрытое поле
        if (!string.IsNullOrEmpty(vm.Website))
            return Ok(new { success = true }); // тихо игнорируем

        if (!ModelState.IsValid)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        var lead = new Lead
        {
            CarId = vm.CarId,
            Name = vm.Name.Trim(),
            Phone = vm.Phone.Trim(),
            LeadType = vm.LeadType,
            Message = vm.Message?.Trim(),
            IP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
        };

        var id = await _leads.CreateAsync(lead);

        // Загружаем с авто для уведомления
        var full = await _leads.GetByIdAsync(id);
        if (full != null)
        {
            var result = await _notify.SendLeadAsync(full);

            // Помечаем только реальную ошибку отправки, не "не настроен"
            if (result == NotifyResult.Failed)
                await _leads.MarkNotifyFailedAsync(id);
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { success = true });

        TempData["LeadSuccess"] = true;
        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
    }
}