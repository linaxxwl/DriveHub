using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Domain.Entities;

// Автомобиль в каталоге салона
public class Car
{
    public int CarId { get; set; }
    public int ModelId { get; set; }
    public CarModel? Model { get; set; }
    public string Vin { get; set; } = string.Empty;
    public int ManufactureYear { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string EngineType { get; set; } = string.Empty;
    public decimal EngineVolume { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string DriveType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CarCondition Condition { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public CarStatus Status { get; set; } = CarStatus.InStock;
    public DateTime ArrivalDate { get; set; } = DateTime.UtcNow;

    // Резервирует автомобиль. Допустимо только из статуса «В наличии»
    public void Reserve()
    {
        if (Status != CarStatus.InStock)
            throw new InvalidOperationException("Автомобиль недоступен для резерва.");
        Status = CarStatus.Reserved;
    }

    // Снимает резерв (при отмене или отклонении заявки)
    public void Release()
    {
        if (Status == CarStatus.Reserved) Status = CarStatus.InStock;
    }

    // Помечает автомобиль проданным
    public void MarkSold()
    {
        if (Status == CarStatus.Sold || Status == CarStatus.Withdrawn)
            throw new InvalidOperationException("Автомобиль нельзя продать.");
        Status = CarStatus.Sold;
    }

    // Снимает автомобиль с продажи
    public void Withdraw() => Status = CarStatus.Withdrawn;
}