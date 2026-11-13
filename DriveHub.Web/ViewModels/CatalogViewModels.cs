using DriveHub.Web.Domain.Entities;

namespace DriveHub.Web.ViewModels;

/// <summary>Параметры поиска и фильтрации каталога (приходят из адресной строки).</summary>
public class CatalogFilter
{
    /// <summary>Выбранная марка (null — любая).</summary>
    public int? BrandId { get; set; }

    /// <summary>Максимальная цена (null — без ограничения).</summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>Минимальный год выпуска (null — любой).</summary>
    public int? MinYear { get; set; }

    /// <summary>Способ сортировки: new, price_asc, price_desc, year_desc.</summary>
    public string Sort { get; set; } = "new";

    /// <summary>Номер страницы (с единицы).</summary>
    public int Page { get; set; } = 1;
}

/// <summary>Данные для страницы каталога.</summary>
public class CatalogViewModel
{
    /// <summary>Автомобили текущей страницы.</summary>
    public List<Car> Cars { get; set; } = new();

    /// <summary>Список марок для выпадающего фильтра.</summary>
    public List<Brand> Brands { get; set; } = new();

    /// <summary>Текущие параметры фильтра (чтобы форма запоминала выбор).</summary>
    public CatalogFilter Filter { get; set; } = new();

    /// <summary>Номер текущей страницы.</summary>
    public int Page { get; set; }

    /// <summary>Всего страниц.</summary>
    public int Pages { get; set; }

    /// <summary>Всего найдено автомобилей.</summary>
    public int Total { get; set; }
}