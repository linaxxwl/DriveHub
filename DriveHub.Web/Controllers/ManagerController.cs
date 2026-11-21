using System.Security.Claims;
using DriveHub.Web.Domain.Enums;
using DriveHub.Web.Services;
using DriveHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers;

/// <summary>Кабинет менеджера: обработка заявок. Доступен менеджеру и администратору.</summary>
[Authorize(Roles = "Менеджер,Администратор")]
public class ManagerController : Controller
{
    private readonly ManagerService _manager;

    /// <summary>Создает контроллер.</summary>
    /// <param name="manager">Сервис менеджера.</param>
    public ManagerController(ManagerService manager) => _manager = manager;

    /// <summary>Идентификатор текущего сотрудника из cookie.</summary>
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Список заявок. Без параметра показывает очередь («Новая» и «В обработке»).</summary>
    /// <param name="status">Статус для фильтра.</param>
    public async Task<IActionResult> Index(RequestStatus? status) =>
        View(new ManagerQueueViewModel
        {
            Status = status,
            Requests = await _manager.GetQueueAsync(status)
        });

    /// <summary>Берет заявку в работу.</summary>
    /// <param name="id">Идентификатор заявки.</param>
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> TakeInWork(int id) =>
        RunAsync(() => _manager.TakeInWorkAsync(id, UserId), "Заявка взята в работу.");

    /// <summary>Одобряет заявку (для покупки автомобиль резервируется).</summary>
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Approve(int id) =>
        RunAsync(() => _manager.ApproveAsync(id, UserId), "Заявка одобрена.");

    /// <summary>Отклоняет заявку с комментарием.</summary>
    /// <param name="comment">Причина отклонения.</param>
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Reject(int id, string? comment) =>
        RunAsync(() => _manager.RejectAsync(id, UserId, comment), "Заявка отклонена.");

    /// <summary>
    /// Выполняет действие и возвращает на список заявок. Нарушение правил (неверный статус,
    /// пустой комментарий, автомобиль занят) показывается сообщением, а не ошибкой сервера.
    /// </summary>
    /// <param name="action">Действие менеджера.</param>
    /// <param name="successMessage">Сообщение при успехе.</param>
    private async Task<IActionResult> RunAsync(Func<Task> action, string successMessage)
    {
        try
        {
            await action();
            TempData["Message"] = successMessage;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            TempData["Message"] = ex is ArgumentException ae ? ae.Message.Split('\n')[0] : ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}