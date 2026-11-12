using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.Services.Facades;

/// <summary>Фасад оформления сделок: скрывает от контроллеров работу с заявкой, автомобилем и журналом.</summary>
public interface IDealFacade
{
    /// <summary>Оформляет сделку по одобренной заявке одной транзакцией.</summary>
    /// <param name="requestId">Идентификатор заявки.</param>
    /// <param name="managerId">Идентификатор менеджера.</param>
    /// <param name="paymentMethod">Способ оплаты.</param>
    /// <param name="discount">Скидка в процентах (0–100).</param>
    Task<Deal> CreateDealAsync(int requestId, int managerId, PaymentMethod paymentMethod, decimal discount);

    /// <summary>Отмечает сделку оплаченной.</summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="managerId">Идентификатор менеджера.</param>
    Task MarkPaidAsync(int dealId, int managerId);

    /// <summary>Расторгает сделку и возвращает автомобиль в продажу.</summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="managerId">Идентификатор менеджера.</param>
    Task CancelDealAsync(int dealId, int managerId);
}