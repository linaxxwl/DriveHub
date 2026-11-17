using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services;

/// <summary>Работа с избранными автомобилями клиента.</summary>
public class FavoritesService
{
    private readonly AppDbContext _db;

    /// <summary>Создает сервис.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public FavoritesService(AppDbContext db) => _db = db;

    /// <summary>Проверяет, есть ли автомобиль в избранном у клиента.</summary>
    /// <param name="userId">Идентификатор клиента.</param>
    /// <param name="carId">Идентификатор автомобиля.</param>
    public Task<bool> IsFavoriteAsync(int userId, int carId) =>
        _db.Favorites.AnyAsync(f => f.ClientId == userId && f.CarId == carId);

    /// <summary>Добавляет автомобиль в избранное, а если он уже там, убирает. Возвращает true, если после вызова он в избранном.</summary>
    /// <param name="userId">Идентификатор клиента.</param>
    /// <param name="carId">Идентификатор автомобиля.</param>
    public async Task<bool> ToggleAsync(int userId, int carId)
    {
        // Ключ избранного составной: сначала ClientId, потом CarId 
        var favorite = await _db.Favorites.FindAsync(userId, carId);

        if (favorite != null)
        {
            // Уже в избранном: убираем
            _db.Favorites.Remove(favorite);
            _db.ActionLogs.Add(new ActionLog { UserId = userId, ActionType = "Удаление из избранного", Description = $"Автомобиль №{carId}" });
            await _db.SaveChangesAsync();
            return false;
        }

        // Защита: нельзя добавить автомобиль, которого нет в базе
        if (!await _db.Cars.AnyAsync(c => c.CarId == carId))
            throw new InvalidOperationException("Автомобиль не найден.");

        _db.Favorites.Add(new Favorite { ClientId = userId, CarId = carId });
        _db.ActionLogs.Add(new ActionLog { UserId = userId, ActionType = "Добавление в избранное", Description = $"Автомобиль №{carId}" });
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>Возвращает автомобили из избранного клиента.</summary>
    /// <param name="userId">Идентификатор клиента.</param>
    public Task<List<Car>> GetCarsAsync(int userId) =>
        _db.Cars
            .Include(c => c.Model).ThenInclude(m => m!.Brand)
            .Where(c => _db.Favorites.Any(f => f.ClientId == userId && f.CarId == c.CarId))
            .OrderByDescending(c => c.ArrivalDate)
            .ToListAsync();
}