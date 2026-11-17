using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.ViewModels;

/// <summary>Русские названия перечислений для показа на страницах.</summary>
public static class DisplayNames
{
    /// <summary>Название статуса заявки.</summary>
    /// <param name="status">Статус заявки.</param>
    public static string ToRu(this RequestStatus status) => status switch
    {
        RequestStatus.New => "Новая",
        RequestStatus.InProgress => "В обработке",
        RequestStatus.Approved => "Одобрена",
        RequestStatus.Rejected => "Отклонена",
        RequestStatus.Cancelled => "Отменена",
        RequestStatus.Closed => "Закрыта",
        _ => status.ToString()
    };

    /// <summary>Название типа заявки.</summary>
    /// <param name="type">Тип заявки.</param>
    public static string ToRu(this RequestType type) => type switch
    {
        RequestType.Purchase => "Покупка",
        RequestType.TestDrive => "Тест-драйв",
        _ => type.ToString()
    };
}