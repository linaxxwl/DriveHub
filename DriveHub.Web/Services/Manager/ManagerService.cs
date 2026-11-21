using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services;

/// <summary>Работа менеджера с заявками: очередь, взять в работу, одобрить, отклонить.</summary>
public class ManagerService
{
    private readonly AppDbContext _db;

    /// <summary>Создает сервис.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public ManagerService(AppDbContext db) => _db = db;

    /// <summary>Возвращает заявки для менеджера: по умолчанию новые и в обработке, иначе с выбранным статусом.</summary>
    /// <param name="status">Статус для фильтра или null (очередь: «Новая» и «В обработке»).</param>
    public Task<List<Request>> GetQueueAsync(RequestStatus? status)
    {
        var query = _db.Requests
            .Include(r => r.Client)
            .Include(r => r.Manager)
            .Include(r => r.Car).ThenInclude(c => c!.Model).ThenInclude(m => m!.Brand)
            .AsQueryable();

        query = status.HasValue
            ? query.Where(r => r.Status == status.Value)
            : query.Where(r => r.Status == RequestStatus.New || r.Status == RequestStatus.InProgress);

        // Сначала самые старые: их нужно обработать в первую очередь
        return query.OrderBy(r => r.CreatedAt).ToListAsync();
    }

    /// <summary>Берет заявку в работу и закрепляет ее за менеджером.</summary>
    /// <param name="requestId">Идентификатор заявки.</param>
    /// <param name="managerId">Идентификатор менеджера.</param>
    public async Task TakeInWorkAsync(int requestId, int managerId)
    {
        var request = await FindAsync(requestId);
        request.TakeInWork(managerId); // переход по паттерну «Состояние»

        _db.ActionLogs.Add(CreateLog(managerId, "Заявка взята в работу", $"Заявка №{requestId}"));
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Одобряет заявку. Для заявки на покупку автомобиль резервируется,
    /// а конкурирующие заявки на этот автомобиль отклоняются. Все сохраняется одним вызовом SaveChanges.
    /// </summary>
    public async Task ApproveAsync(int requestId, int managerId)
    {
        var request = await FindAsync(requestId);
        request.Approve(); // если статус не позволяет, выбросит исключение

        if (request.Type == RequestType.Purchase)
        {
            request.Car!.Reserve(); // если автомобиль уже занят, выбросит исключение
            await RejectCompetingAsync(request, managerId);
        }

        _db.ActionLogs.Add(CreateLog(managerId, "Заявка одобрена", $"Заявка №{requestId}, автомобиль №{request.CarId}"));
        await _db.SaveChangesAsync();
    }

    /// <summary>Отклоняет заявку с обязательным комментарием.</summary>
    /// <param name="comment">Причина отклонения.</param>
    public async Task RejectAsync(int requestId, int managerId, string? comment)
    {
        var request = await FindAsync(requestId);
        request.Reject(comment?.Trim() ?? string.Empty); // пустой комментарий state не пропустит

        _db.ActionLogs.Add(CreateLog(managerId, "Заявка отклонена", $"Заявка №{requestId}: {request.ManagerComment}"));
        await _db.SaveChangesAsync();
    }

    /// <summary>Отклоняет остальные активные заявки на покупку того же автомобиля.</summary>
    /// <param name="approved">Одобренная заявка.</param>
    private async Task RejectCompetingAsync(Request approved, int managerId)
    {
        var competitors = await _db.Requests
            .Where(r => r.CarId == approved.CarId
                        && r.RequestId != approved.RequestId
                        && r.Type == RequestType.Purchase
                        && (r.Status == RequestStatus.New || r.Status == RequestStatus.InProgress))
            .ToListAsync();

        foreach (var other in competitors)
        {
            // Отклонить можно только заявку «В обработке», поэтому новую сначала берем в работу
            if (other.Status == RequestStatus.New) other.TakeInWork(managerId);
            other.Reject("Автомобиль зарезервирован за другим клиентом.");
            _db.ActionLogs.Add(CreateLog(managerId, "Заявка отклонена автоматически",
                $"Заявка №{other.RequestId}: автомобиль №{other.CarId} зарезервирован по заявке №{approved.RequestId}"));
        }
    }

    /// <summary>Находит заявку вместе с автомобилем или выбрасывает исключение.</summary>
    private async Task<Request> FindAsync(int requestId) =>
        await _db.Requests.Include(r => r.Car).FirstOrDefaultAsync(r => r.RequestId == requestId)
            ?? throw new InvalidOperationException("Заявка не найдена.");

    /// <summary>Создает запись журнала действий.</summary>
    /// <param name="userId">Кто выполнил действие.</param>
    /// <param name="actionType">Тип действия.</param>
    /// <param name="description">Описание.</param>
    private static ActionLog CreateLog(int userId, string actionType, string description) =>
        new() { UserId = userId, ActionType = actionType, Description = description };
}