using DriveHub.Web.Domain.Enums;
using DriveHub.Web.Services.States;

namespace DriveHub.Web.Domain.Entities;

//Заявка клиента на покупку или тест-драйв
public class Request
{
    public int RequestId { get; set; }
    public int ClientId { get; set; }
    public User? Client { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int? ManagerId { get; set; }
    public User? Manager { get; set; }
    public RequestType Type { get; set; }
    public DateTime DesiredDate { get; set; }
    public TimeSpan? TestDriveTime { get; set; }
    public string ClientComment { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ManagerComment { get; set; }

    private RequestState State => RequestStateFactory.Create(Status);

    /// <summary>Берет заявку в работу.</summary>
    /// <param name="managerId">Идентификатор менеджера.</param>
    public void TakeInWork(int managerId) => State.TakeInWork(this, managerId);

    /// <summary>Одобряет заявку.</summary>
    public void Approve() => State.Approve(this);

    /// <summary>Отклоняет заявку с комментарием.</summary>
    /// <param name="comment">Причина отклонения.</param>
    public void Reject(string comment) => State.Reject(this, comment);

    /// <summary>Отменяет заявку.</summary>
    public void Cancel() => State.Cancel(this);
}