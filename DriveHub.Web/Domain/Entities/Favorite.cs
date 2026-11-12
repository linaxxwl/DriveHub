namespace DriveHub.Web.Domain.Entities;

/// <summary>Избранный автомобиль клиента (составной ключ ClientId + CarId)</summary>
public class Favorite
{
    public int ClientId { get; set; }
    public User? Client { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}