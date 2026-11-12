using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Services.States;

/// <summary>Состояние «Одобрена»: можно только отменить (до оформления сделки).</summary>
public class ApprovedState : RequestState
{
    /// <inheritdoc />
    public override void Cancel(Request request) => request.Status = RequestStatus.Cancelled;
}