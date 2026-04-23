using System.ComponentModel.DataAnnotations;

namespace AutoSalon.ViewModels;

public class RegisterViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [Display(Name = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = "";
}