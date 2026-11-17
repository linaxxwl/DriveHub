using System.Security.Claims;
using DriveHub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Избранное. Доступно только клиентам: гость попадет на страницу входа.</summary>
[Authorize(Roles = "Клиент")]
public class FavoritesController : Controller
{
    private readonly FavoritesService _favorites;

    /// <summary>Создает контроллер.</summary>
    /// <param name="favorites">Сервис избранного.</param>
    public FavoritesController(FavoritesService favorites) => _favorites = favorites;

    /// <summary>Идентификатор текущего пользователя (записывается в cookie при входе).</summary>
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Страница «Избранное».</summary>
    public async Task<IActionResult> Index() =>
        View(await _favorites.GetCarsAsync(UserId));

    /// <summary>Добавляет или убирает автомобиль и возвращает пользователя на прежнюю страницу.</summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    /// <param name="returnUrl">Адрес страницы, с которой пришли.</param>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int carId, string? returnUrl)
    {
        await _favorites.ToggleAsync(UserId, carId);

        // Url.IsLocalUrl защищает от перенаправления на чужой сайт
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }
}