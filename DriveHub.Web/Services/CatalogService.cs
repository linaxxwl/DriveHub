using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using DriveHub.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services;

/// <summary>Поиск, фильтрация и сортировка автомобилей каталога.</summary>
public class CatalogService
{
    /// <summary>Сколько автомобилей показывать на одной странице.</summary>
    public const int PageSize = 6;

    private readonly AppDbContext _db;

    /// <summary>Создает сервис.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public CatalogService(AppDbContext db) => _db = db;

    /// <summary>Возвращает страницу каталога с учётом фильтра и сортировки.</summary>
    /// <param name="filter">Параметры фильтра, сортировки и страницы.</param>
    public async Task<CatalogViewModel> GetCatalogAsync(CatalogFilter filter)
    {
        // Базовый запрос: в каталоге видны только автомобили «В наличии» и «Зарезервирован»
        var query = _db.Cars
            .Include(c => c.Model).ThenInclude(m => m!.Brand)
            .Where(c => c.Status == CarStatus.InStock || c.Status == CarStatus.Reserved);

        // Фильтры (добавляем условие, только если значение задано)
        if (filter.BrandId.HasValue)
            query = query.Where(c => c.Model!.BrandId == filter.BrandId.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= filter.MaxPrice.Value);
        if (filter.MinYear.HasValue)
            query = query.Where(c => c.ManufactureYear >= filter.MinYear.Value);

        // Сортировка
        query = filter.Sort switch
        {
            "price_asc" => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "year_desc" => query.OrderByDescending(c => c.ManufactureYear),
            _ => query.OrderByDescending(c => c.ArrivalDate) // по умолчанию: новые поступления
        };

        // Постраничный вывод
        int total = await query.CountAsync();
        int pages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        int page = Math.Clamp(filter.Page, 1, pages); // защита от неверного номера страницы

        var cars = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        return new CatalogViewModel
        {
            Cars = cars,
            Brands = await _db.Brands.OrderBy(b => b.Name).ToListAsync(),
            Filter = filter,
            Page = page,
            Pages = pages,
            Total = total
        };
    }

    /// <summary>Возвращает автомобиль для карточки или null, если не найден.</summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    public Task<Car?> GetCarAsync(int id) =>
        _db.Cars
            .Include(c => c.Model).ThenInclude(m => m!.Brand)
            .FirstOrDefaultAsync(c => c.CarId == id);
}