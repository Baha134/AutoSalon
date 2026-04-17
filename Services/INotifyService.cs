using AutoSalon.Models;

namespace AutoSalon.Services;

public interface INotifyService
{
    Task<bool> SendLeadAsync(Lead lead);
}