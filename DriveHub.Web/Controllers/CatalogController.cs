using System.Security.Claims;
using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Каталог автомобилей: список с фильтрами и карточка. Доступен всем, в том числе гостям.</summary>
public class CatalogController : Controller
{
    private readonly CatalogService _catalog;
    private readonly FavoritesService _favorites;

    /// <summary>Создает контроллер.</summary>
    /// <param name="catalog">Сервис каталога.</param>
    /// <param name="favorites">Сервис избранного (нужен, чтобы подписать кнопку на карточке).</param>
    public CatalogController(CatalogService catalog, FavoritesService favorites)
    {
        _catalog = catalog;
        _favorites = favorites;
    }

    /// <summary>Страница каталога. Параметры фильтра берутся из адреса (?BrandId=1&amp;Sort=price_asc).</summary>
    /// <param name="filter">Параметры фильтра.</param>
    public async Task<IActionResult> Index(CatalogFilter filter) =>
        View(await _catalog.GetCatalogAsync(filter));

    /// <summary>Карточка автомобиля.</summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    public async Task<IActionResult> Details(int id)
    {
        var car = await _catalog.GetCarAsync(id);
        if (car == null) return NotFound(); // такого автомобиля нет

        // Для клиента узнаем, в избранном ли автомобиль (от этого зависит надпись на кнопке)
        ViewBag.IsFavorite = false;
        if (User.IsInRole("Клиент"))
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ViewBag.IsFavorite = await _favorites.IsFavoriteAsync(userId, id);
        }
        return View(car);
    }
}