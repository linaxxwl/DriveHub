namespace DriveHub.Web.Domain.Entities;

// Модель автомобиля
public class CarModel
{
    public int ModelId { get; set; }
    public int BrandId { get; set; }
    public Brand? Brand { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public int ProductionStartYear { get; set; }
}