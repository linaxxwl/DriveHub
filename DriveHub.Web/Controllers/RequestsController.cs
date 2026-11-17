using System.Security.Claims;
using DriveHub.Web.Domain.Enums;
using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Заявки клиента: подача, список и отмена. Только для роли «Клиент».</summary>
[Authorize(Roles = "Клиент")]
public class RequestsController : Controller
{
    private readonly RequestService _requests;
    private readonly CatalogService _catalog;

    /// <summary>Создает контроллер.</summary>
    /// <param name="requests">Сервис заявок.</param>
    /// <param name="catalog">Сервис каталога (название автомобиля для формы).</param>
    public RequestsController(RequestService requests, CatalogService catalog)
    {
        _requests = requests;
        _catalog = catalog;
    }

    /// <summary>Идентификатор текущего пользователя из cookie.</summary>
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Форма подачи заявки.</summary>
    /// <param name="carId">Автомобиль.</param>
    /// <param name="type">Тип заявки (по умолчанию покупка).</param>
    [HttpGet]
    public async Task<IActionResult> Create(int carId, RequestType type = RequestType.Purchase)
    {
        var car = await _catalog.GetCarAsync(carId);
        if (car == null) return NotFound();

        return View(new CreateRequestViewModel
        {
            CarId = carId,
            Type = type,
            CarName = $"{car.Model!.Brand!.Name} {car.Model.Name}"
        });
    }

    /// <summary>Обработка отправленной формы.</summary>
    /// <param name="model">Данные формы.</param>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRequestViewModel model)
    {
        var car = await _catalog.GetCarAsync(model.CarId);
        if (car == null) return NotFound();
        model.CarName = $"{car.Model!.Brand!.Name} {car.Model.Name}";

        if (!ModelState.IsValid) return View(model);

        try
        {
            await _requests.CreateAsync(UserId, model.CarId, model.Type,
                model.DesiredDate, model.TestDriveTime, model.ClientComment);
            TempData["Message"] = "Заявка отправлена. Менеджер рассмотрит её в ближайшее время.";
            return RedirectToAction(nameof(My));
        }
        catch (InvalidOperationException ex)
        {
            // Нарушено бизнес-правило: показываем причину на форме
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    /// <summary>Страница «Мои заявки».</summary>
    public async Task<IActionResult> My() =>
        View(await _requests.GetClientRequestsAsync(UserId));

    /// <summary>Отмена заявки.</summary>
    /// <param name="id">Идентификатор заявки.</param>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _requests.CancelAsync(id, UserId);
            TempData["Message"] = "Заявка отменена.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Message"] = ex.Message;
        }
        return RedirectToAction(nameof(My));
    }
}