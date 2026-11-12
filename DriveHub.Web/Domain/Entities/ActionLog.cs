namespace DriveHub.Web.Domain.Entities;

/// <summary>Запись журнала действий пользователя</summary>
public class ActionLog
{
    public int LogId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
}