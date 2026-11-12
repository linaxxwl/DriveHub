using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services.Facades;

/// <summary>Реализация фасада сделок. Все операции выполняются в транзакции.</summary>
public class DealFacade : IDealFacade
{
    private readonly AppDbContext _db;

    /// <summary>Создает фасад.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public DealFacade(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<Deal> CreateDealAsync(int requestId, int managerId, PaymentMethod paymentMethod, decimal discount)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var request = await _db.Requests.Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.RequestId == requestId)
                ?? throw new InvalidOperationException("Заявка не найдена.");

            bool dealExists = await _db.Deals.AnyAsync(d => d.RequestId == requestId);
            ValidateRules(request, discount, dealExists);

            var car = request.Car!;
            var deal = new Deal
            {
                RequestId = request.RequestId,
                CarId = car.CarId,
                ClientId = request.ClientId,
                ManagerId = managerId,
                PaymentMethod = paymentMethod,
                Discount = discount,
                ContractNumber = GenerateContractNumber()
            };
            deal.CalculateTotal(car.Price);

            car.MarkSold();
            request.Status = RequestStatus.Closed;

            _db.Deals.Add(deal);
            _db.ActionLogs.Add(CreateLog(managerId, "Оформление сделки",
                $"Заявка №{request.RequestId}, автомобиль №{car.CarId}, договор {deal.ContractNumber}"));

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return deal;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task MarkPaidAsync(int dealId, int managerId)
    {
        var deal = await _db.Deals.FindAsync(dealId)
            ?? throw new InvalidOperationException("Сделка не найдена.");
        if (deal.Status != DealStatus.Created)
            throw new InvalidOperationException("Оплатить можно только созданную сделку.");

        deal.MarkPaid();
        _db.ActionLogs.Add(CreateLog(managerId, "Оплата сделки", $"Договор {deal.ContractNumber}"));
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task CancelDealAsync(int dealId, int managerId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var deal = await _db.Deals.Include(d => d.Car).FirstOrDefaultAsync(d => d.DealId == dealId)
                ?? throw new InvalidOperationException("Сделка не найдена.");
            if (deal.Status == DealStatus.Cancelled)
                throw new InvalidOperationException("Сделка уже расторгнута.");

            deal.Cancel();
            deal.Car!.Status = CarStatus.InStock;
            _db.ActionLogs.Add(CreateLog(managerId, "Расторжение сделки", $"Договор {deal.ContractNumber}"));

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>Проверяет бизнес-правила оформления сделки.</summary>
    /// <param name="request">Заявка.</param>
    /// <param name="discount">Скидка в процентах.</param>
    /// <param name="dealExists">Есть ли уже сделка по этой заявке.</param>
    private static void ValidateRules(Request request, decimal discount, bool dealExists)
    {
        if (request.Type != RequestType.Purchase)
            throw new InvalidOperationException("Сделку можно оформить только по заявке на покупку.");
        if (request.Status != RequestStatus.Approved)
            throw new InvalidOperationException("Заявка должна быть одобрена.");
        if (request.Car == null || request.Car.Status != CarStatus.Reserved)
            throw new InvalidOperationException("Автомобиль должен быть зарезервирован.");
        if (discount < 0 || discount > 100)
            throw new ArgumentOutOfRangeException(nameof(discount), "Скидка должна быть от 0 до 100%.");
        if (dealExists)
            throw new InvalidOperationException("По этой заявке сделка уже оформлена.");
    }

    /// <summary>Формирует уникальный номер договора.</summary>
    private static string GenerateContractNumber() =>
        $"DH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    /// <summary>Создаёт запись журнала действий.</summary>
    /// <param name="userId">Кто выполнил действие.</param>
    /// <param name="actionType">Тип действия.</param>
    /// <param name="description">Описание.</param>
    private static ActionLog CreateLog(int userId, string actionType, string description) =>
        new() { UserId = userId, ActionType = actionType, Description = description };
}