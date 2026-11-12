using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Domain.Entities;

/// <summary>Сделка купли-продажи</summary>
public class Deal
{
    public int DealId { get; set; }
    public int RequestId { get; set; }
    public Request? Request { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int ClientId { get; set; }
    public User? Client { get; set; }
    public int ManagerId { get; set; }
    public User? Manager { get; set; }
    public DateTime DealDate { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }
    public decimal Discount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Created;
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>Рассчитывает итоговую цену с учетом скидки (в процентах)</summary>
    /// <param name="basePrice">Цена автомобиля</param>
    public void CalculateTotal(decimal basePrice) =>
        TotalPrice = Math.Round(basePrice * (1 - Discount / 100m), 2);

    /// <summary>Отмечает сделку оплаченной</summary>
    public void MarkPaid() => Status = DealStatus.Paid;

    /// <summary>Расторгает сделку</summary>
    public void Cancel() => Status = DealStatus.Cancelled;
}