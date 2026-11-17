using System.ComponentModel.DataAnnotations;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Web.ViewModels;

/// <summary>Данные формы подачи заявки.</summary>
public class CreateRequestViewModel
{
    /// <summary>Автомобиль, по которому подается заявка.</summary>
    public int CarId { get; set; }

    /// <summary>Тип заявки: покупка или тест-драйв.</summary>
    public RequestType Type { get; set; }

    /// <summary>Желаемая дата визита или тест-драйва.</summary>
    [Required(ErrorMessage = "Укажите дату")]
    [DataType(DataType.Date)]
    public DateTime DesiredDate { get; set; } = DateTime.Today.AddDays(1);

    /// <summary>Время тест-драйва (только для заявки на тест-драйв).</summary>
    public TimeSpan? TestDriveTime { get; set; }

    /// <summary>Комментарий клиента.</summary>
    [StringLength(500, ErrorMessage = "Не более 500 символов")]
    public string? ClientComment { get; set; }

    /// <summary>Название автомобиля для заголовка формы (не вводится пользователем).</summary>
    public string? CarName { get; set; }
}