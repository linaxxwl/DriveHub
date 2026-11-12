using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Services.States;

/// <summary>Состояние «Новая»: можно взять в работу или отменить.</summary>
public class NewState : RequestState
{
    /// <inheritdoc />
    public override void TakeInWork(Request request, int managerId)
    {
        request.ManagerId = managerId;
        request.Status = RequestStatus.InProgress;
    }

    /// <inheritdoc />
    public override void Cancel(Request request) => request.Status = RequestStatus.Cancelled;
}