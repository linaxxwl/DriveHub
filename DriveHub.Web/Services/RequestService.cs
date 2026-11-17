using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services;

/// <summary>Заявки клиента: создание с проверками, список и отмена.</summary>
public class RequestService
{
    /// <summary>Статусы, в которых заявка считается «активной».</summary>
    private static readonly RequestStatus[] ActiveStatuses =
        { RequestStatus.New, RequestStatus.InProgress, RequestStatus.Approved };

    /// <summary>Начало и конец рабочего дня салона (для тест-драйвов).</summary>
    private static readonly TimeSpan WorkStart = new(9, 0, 0);
    private static readonly TimeSpan WorkEnd = new(20, 0, 0);

    private readonly AppDbContext _db;

    /// <summary>Создает сервис.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public RequestService(AppDbContext db) => _db = db;

    /// <summary>Создает заявку. Если правила нарушены, выбрасывает InvalidOperationException с понятным текстом.</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="carId">Идентификатор автомобиля.</param>
    /// <param name="type">Тип заявки.</param>
    /// <param name="desiredDate">Желаемая дата.</param>
    /// <param name="testDriveTime">Время тест-драйва (для покупки не нужно).</param>
    /// <param name="comment">Комментарий клиента.</param>
    public async Task<Request> CreateAsync(int clientId, int carId, RequestType type,
        DateTime desiredDate, TimeSpan? testDriveTime, string? comment)
    {
        // Правило 1: автомобиль должен быть в наличии
        var car = await _db.Cars.FindAsync(carId)
            ?? throw new InvalidOperationException("Автомобиль не найден.");
        if (car.Status != CarStatus.InStock)
            throw new InvalidOperationException("Автомобиль недоступен: он зарезервирован, продан или снят с продажи.");

        // Правило 2: у клиента не должно быть активной заявки того же типа на этот автомобиль
        bool duplicate = await _db.Requests.AnyAsync(r =>
            r.ClientId == clientId && r.CarId == carId && r.Type == type && ActiveStatuses.Contains(r.Status));
        if (duplicate)
            throw new InvalidOperationException("У вас уже есть активная заявка такого типа на этот автомобиль.");

        // Правила по дате и времени
        if (type == RequestType.TestDrive)
            await ValidateTestDriveAsync(carId, desiredDate, testDriveTime);
        else if (desiredDate.Date < DateTime.Today)
            throw new InvalidOperationException("Дата визита не может быть в прошлом.");

        var request = new Request
        {
            ClientId = clientId,
            CarId = carId,
            Type = type,
            DesiredDate = desiredDate.Date,
            TestDriveTime = type == RequestType.TestDrive ? testDriveTime : null,
            ClientComment = comment?.Trim() ?? string.Empty
        };
        _db.Requests.Add(request);
        _db.ActionLogs.Add(new ActionLog
        {
            UserId = clientId,
            ActionType = "Создание заявки",
            Description = $"{(type == RequestType.Purchase ? "Покупка" : "Тест-драйв")}, автомобиль №{carId}"
        });
        await _db.SaveChangesAsync();
        return request;
    }

    /// <summary>Проверяет время тест-драйва: задано, в рабочие часы, не в прошлом и свободно.</summary>
    /// <param name="carId">Автомобиль.</param>
    /// <param name="date">Дата тест-драйва.</param>
    /// <param name="time">Время тест-драйва.</param>
    private async Task ValidateTestDriveAsync(int carId, DateTime date, TimeSpan? time)
    {
        if (time == null)
            throw new InvalidOperationException("Укажите время тест-драйва.");
        if (time < WorkStart || time > WorkEnd)
            throw new InvalidOperationException("Тест-драйвы проводятся с 9:00 до 20:00.");
        if (date.Date + time.Value <= DateTime.Now)
            throw new InvalidOperationException("Время тест-драйва уже прошло.");

        // Время занято, если на этот автомобиль уже есть активный тест-драйв на то же время
        bool busy = await _db.Requests.AnyAsync(r =>
            r.CarId == carId && r.Type == RequestType.TestDrive &&
            ActiveStatuses.Contains(r.Status) &&
            r.DesiredDate == date.Date && r.TestDriveTime == time);
        if (busy)
            throw new InvalidOperationException("Это время уже занято. Выберите другое.");
    }

    /// <summary>Возвращает заявки клиента (сначала новые).</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    public Task<List<Request>> GetClientRequestsAsync(int clientId) =>
        _db.Requests
            .Include(r => r.Car).ThenInclude(c => c!.Model).ThenInclude(m => m!.Brand)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    /// <summary>Отменяет заявку клиента. Чужую заявку отменить нельзя: она просто не находится.</summary>
    /// <param name="requestId">Идентификатор заявки.</param>
    /// <param name="clientId">Идентификатор клиента.</param>
    public async Task CancelAsync(int requestId, int clientId)
    {
        var request = await _db.Requests.Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.RequestId == requestId && r.ClientId == clientId)
            ?? throw new InvalidOperationException("Заявка не найдена.");

        bool wasApproved = request.Status == RequestStatus.Approved;

        request.Cancel(); // переход по паттерну «Состояние»; из конечных статусов выбросит исключение

        // Если заявка на покупку была одобрена, автомобиль был зарезервирован: снимаем резерв
        if (wasApproved && request.Type == RequestType.Purchase)
            request.Car?.Release();

        _db.ActionLogs.Add(new ActionLog
        {
            UserId = clientId,
            ActionType = "Отмена заявки",
            Description = $"Заявка №{requestId}"
        });
        await _db.SaveChangesAsync();
    }
}