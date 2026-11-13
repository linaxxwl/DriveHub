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
    [RegularExpression(@"^(\+7|8)\d{10}$", ErrorMessage = "Телефон: +79991234567 или 89991234567")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "Пароль от 6 до 64 символов")]
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