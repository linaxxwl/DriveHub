using System.Diagnostics;
using DriveHub.Web.Models;
using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Главная страница, «О проекте» и страница ошибки.</summary>
public class HomeController : Controller
{
    private readonly CatalogService _catalog;

    /// <summary>Создает контроллер.</summary>
    /// <param name="catalog">Сервис каталога (нужен для блока «Новые поступления»).</param>
    public HomeController(CatalogService catalog) => _catalog = catalog;

    /// <summary>Главная страница: быстрый поиск и три последних автомобиля.</summary>
    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            Latest = await _catalog.GetLatestAsync(3),
            Brands = await _catalog.GetBrandsAsync()
        };
        return View(model);
    }

    /// <summary>Страница «О проекте» с темой работы и исполнителем.</summary>
    public IActionResult About() => View();

    /// <summary>Страница ошибки.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}