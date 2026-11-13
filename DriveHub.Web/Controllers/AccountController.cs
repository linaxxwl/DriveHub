using System.Security.Claims;
using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Регистрация, вход и выход.</summary>
public class AccountController : Controller
{
    private readonly AccountService _accounts;

    /// <summary>Создает контроллер.</summary>
    /// <param name="accounts">Сервис аккаунтов.</param>
    public AccountController(AccountService accounts) => _accounts = accounts;

    [HttpGet] public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _accounts.RegisterAsync(model.FullName, model.Email, model.Phone, model.Password);
        if (user == null)
        {
            ModelState.AddModelError(nameof(model.Email), "Этот e-mail уже зарегистрирован");
            return View(model);
        }
        return RedirectToAction(nameof(Login));
    }

    [HttpGet] public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _accounts.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "Неверный e-mail или пароль");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role!.Name)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}