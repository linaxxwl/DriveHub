using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Каталог автомобилей: список с фильтрами и карточка автомобиля. Доступен всем, в том числе гостям.</summary>
public class CatalogController : Controller
{
    private readonly CatalogService _catalog;

    /// <summary>Создает контроллер.</summary>
    /// <param name="catalog">Сервис каталога.</param>
    public CatalogController(CatalogService catalog) => _catalog = catalog;

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
        return View(car);
    }
}