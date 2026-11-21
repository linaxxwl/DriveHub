using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.ViewModels;

/// <summary>Данные страницы «Заявки» менеджера.</summary>
public class ManagerQueueViewModel
{
    /// <summary>Заявки для показа.</summary>
    public List<Request> Requests { get; set; } = new();

    /// <summary>Выбранный статус фильтра. Null означает очередь: «Новая» и «В обработке».</summary>
    public RequestStatus? Status { get; set; }
}