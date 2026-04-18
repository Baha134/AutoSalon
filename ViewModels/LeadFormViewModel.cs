using AutoSalon.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class LeadFormViewModel
{
    public int? CarId { get; set; }

    [Required(ErrorMessage = "Укажите имя")]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажите телефон")]
    [MaxLength(20)]
    public string Phone { get; set; } = "";

    public LeadType LeadType { get; set; } = LeadType.Call;

    public string? Message { get; set; }

    // Honeypot
    public string? Website { get; set; }
}
