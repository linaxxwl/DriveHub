using DriveHub.Web.Domain.Enums;

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
}