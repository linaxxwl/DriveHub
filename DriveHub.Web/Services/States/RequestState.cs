using DriveHub.Web.Domain.Entities;

namespace DriveHub.Web.Services.States;

/// <summary>Базовое состояние заявки. Действия, не разрешенные в состоянии, выбрасывают исключение.</summary>
public abstract class RequestState
{
    /// <summary>Берет заявку в работу.</summary>
    /// <param name="request">Заявка.</param>
    /// <param name="managerId">Идентификатор менеджера.</param>
    public virtual void TakeInWork(Request request, int managerId) => Deny("взять в работу");

    /// <summary>Одобряет заявку.</summary>
    /// <param name="request">Заявка.</param>
    public virtual void Approve(Request request) => Deny("одобрить");

    /// <summary>Отклоняет заявку.</summary>
    /// <param name="request">Заявка.</param>
    /// <param name="comment">Комментарий менеджера.</param>
    public virtual void Reject(Request request, string comment) => Deny("отклонить");

    /// <summary>Отменяет заявку.</summary>
    /// <param name="request">Заявка.</param>
    public virtual void Cancel(Request request) => Deny("отменить");

    /// <summary>Выбрасывает исключение о недопустимой операции.</summary>
    /// <param name="action">Название действия.</param>
    protected void Deny(string action) =>
        throw new InvalidOperationException($"Нельзя {action} заявку в текущем статусе.");
}