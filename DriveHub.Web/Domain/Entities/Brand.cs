namespace DriveHub.Web.Domain.Entities;

// Марка автомобиля
public class Brand
{
    public int BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
}