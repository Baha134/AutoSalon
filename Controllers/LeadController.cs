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
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // Honeypot — бот заполнил скрытое поле, тихо игнорируем
        if (!string.IsNullOrEmpty(vm.Website))
        {
            if (isAjax)
                return Ok(new { success = true });
            TempData["LeadSuccess"] = true;
            return Redirect(Request.Headers["Referer"].ToString() is { Length: > 0 } r ? r : "/");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // БАГ ИСПРАВЛЕН: вместо BadRequest всегда возвращаем 200 + {success:false, errors}
            // чтобы JS-обработчик корректно показал ошибки пользователю
            if (isAjax)
                return Ok(new { success = false, errors });

            return Redirect(Request.Headers["Referer"].ToString() is { Length: > 0 } r2 ? r2 : "/");
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

        if (isAjax)
            return Ok(new { success = true });

        TempData["LeadSuccess"] = true;
        return Redirect(Request.Headers["Referer"].ToString() is { Length: > 0 } referer ? referer : "/");
    }
}
