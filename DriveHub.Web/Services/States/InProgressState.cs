using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Services.States;

/// <summary>Состояние «В обработке»: можно одобрить, отклонить или отменить.</summary>
public class InProgressState : RequestState
{
    /// <inheritdoc />
    public override void Approve(Request request) => request.Status = RequestStatus.Approved;

    /// <inheritdoc />
    public override void Reject(Request request, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Укажите причину отклонения.", nameof(comment));
        request.ManagerComment = comment;
        request.Status = RequestStatus.Rejected;
    }

    /// <inheritdoc />
    public override void Cancel(Request request) => request.Status = RequestStatus.Cancelled;
}