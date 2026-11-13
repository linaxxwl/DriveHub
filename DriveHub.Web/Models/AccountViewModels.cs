using System.ComponentModel.DataAnnotations;

namespace DriveHub.Web.ViewModels;

/// <summary>Данные формы регистрации.</summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите ФИО")]
    [StringLength(100)]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Введите e-mail")]
    [EmailAddress(ErrorMessage = "Неверный формат e-mail")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Введите телефон")]
    [RegularExpression(@"^\+?[0-9\s\-()]{10,18}$", ErrorMessage = "Неверный формат телефона")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}

/// <summary>Данные формы входа.</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Введите e-mail")]
    [EmailAddress(ErrorMessage = "Неверный формат e-mail")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}