using AutoSalon.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class LeadFormViewModel
{
    public int? CarId { get; set; }

    [Required(ErrorMessage = "Введите имя")]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Введите телефон")]
    [Phone(ErrorMessage = "Неверный формат телефона")]
    public string Phone { get; set; } = "";

    public LeadType LeadType { get; set; } = LeadType.Call;

    public string? Message { get; set; }

    // Honeypot — должно остаться пустым
    public string? Website { get; set; }
}