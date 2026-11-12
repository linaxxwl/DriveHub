using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Services.States;

/// <summary>Создаёт объект состояния по статусу заявки.</summary>
public static class RequestStateFactory
{
    /// <summary>Возвращает состояние, соответствующее статусу.</summary>
    /// <param name="status">Статус заявки.</param>
    public static RequestState Create(RequestStatus status) => status switch
    {
        RequestStatus.New => new NewState(),
        RequestStatus.InProgress => new InProgressState(),
        RequestStatus.Approved => new ApprovedState(),
        _ => new FinalState()
    };
}