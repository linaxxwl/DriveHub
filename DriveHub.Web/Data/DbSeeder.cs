using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using DriveHub.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Data;

/// <summary>Заполняет пустую базу начальными данными (сотрудники и тестовый каталог).</summary>
public static class DbSeeder
{
    /// <summary>Добавляет начальные данные, если их еще нет.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedStaffAsync(db);
        await SeedCatalogAsync(db);
    }

    /// <summary>Создает администратора и менеджера (роли 3 и 2 заданы в AppDbContext).</summary>
    /// <param name="db">Контекст базы данных.</param>
    private static async Task SeedStaffAsync(AppDbContext db)
    {
        // Если сотрудники уже есть, ничего не делаем
        if (await db.Users.AnyAsync(u => u.RoleId != 1)) return;

        db.Users.AddRange(
            new User
            {
                RoleId = 3, // Администратор
                FullName = "Администратор",
                Email = "admin@drivehub.ru",
                Phone = "+79990000001",
                PasswordHash = PasswordHasher.Hash("Admin123!")
            },
            new User
            {
                RoleId = 2, // Менеджер
                FullName = "Менеджер Сергей",
                Email = "manager@drivehub.ru",
                Phone = "+79990000002",
                PasswordHash = PasswordHasher.Hash("Manager123!")
            });
        await db.SaveChangesAsync();
    }

    /// <summary>Создает марки, модели и тестовые автомобили.</summary>
    /// <param name="db">Контекст базы данных.</param>
    private static async Task SeedCatalogAsync(AppDbContext db)
    {
        // Если марки уже есть, каталог не трогаем
        if (await db.Brands.AnyAsync()) return;

        // Марки
        var kia = new Brand { Name = "Kia", Country = "Южная Корея" };
        var toyota = new Brand { Name = "Toyota", Country = "Япония" };
        var hyundai = new Brand { Name = "Hyundai", Country = "Южная Корея" };
        var vw = new Brand { Name = "Volkswagen", Country = "Германия" };

        // Модели (связь с маркой задается через свойство Brand)
        var rio = new CarModel { Brand = kia, Name = "Rio", BodyType = "Седан", ProductionStartYear = 2011 };
        var sportage = new CarModel { Brand = kia, Name = "Sportage", BodyType = "Кроссовер", ProductionStartYear = 1993 };
        var camry = new CarModel { Brand = toyota, Name = "Camry", BodyType = "Седан", ProductionStartYear = 1982 };
        var solaris = new CarModel { Brand = hyundai, Name = "Solaris", BodyType = "Седан", ProductionStartYear = 2010 };
        var tiguan = new CarModel { Brand = vw, Name = "Tiguan", BodyType = "Кроссовер", ProductionStartYear = 2007 };

        // Автомобили (EF сам добавит связанные модели и марки)
        db.Cars.AddRange(
            NewCar(rio, "KIA0000000000001", 2023, "Белый", 0, "Бензин", 1.6m, "Автомат", "Передний", 1_650_000m, CarCondition.New, "Новый Kia Rio в комплектации Comfort."),
            NewCar(rio, "KIA0000000000002", 2019, "Серый", 68_000, "Бензин", 1.6m, "Механика", "Передний", 1_050_000m, CarCondition.Used, "Один владелец, обслуживание у дилера."),
            NewCar(sportage, "KIA0000000000003", 2024, "Чёрный", 0, "Бензин", 2.0m, "Автомат", "Полный", 3_450_000m, CarCondition.New, "Новый Sportage, полный привод."),
            NewCar(camry, "TOY0000000000004", 2022, "Синий", 24_000, "Бензин", 2.5m, "Автомат", "Передний", 3_100_000m, CarCondition.Used, "Состояние нового автомобиля."),
            NewCar(camry, "TOY0000000000005", 2024, "Белый", 0, "Бензин", 2.5m, "Автомат", "Передний", 3_900_000m, CarCondition.New, "Новая Toyota Camry, комплектация Prestige."),
            NewCar(solaris, "HYU0000000000006", 2018, "Красный", 92_000, "Бензин", 1.6m, "Автомат", "Передний", 890_000m, CarCondition.Used, "Экономичный городской седан."),
            NewCar(tiguan, "VW00000000000007", 2021, "Серый", 41_000, "Дизель", 2.0m, "Автомат", "Полный", 2_750_000m, CarCondition.Used, "Дизельный Tiguan, полный привод."),
            NewCar(tiguan, "VW00000000000008", 2024, "Зелёный", 0, "Бензин", 1.4m, "Робот", "Передний", 3_350_000m, CarCondition.New, "Новый Volkswagen Tiguan."));

        await db.SaveChangesAsync();
    }

    /// <summary>Вспомогательный метод: создает автомобиль со всеми характеристиками.</summary>
    private static Car NewCar(CarModel model, string vin, int year, string color, int mileage,
        string engine, decimal volume, string transmission, string drive, decimal price,
        CarCondition condition, string description) => new()
        {
            Model = model,
            Vin = vin,
            ManufactureYear = year,
            Color = color,
            Mileage = mileage,
            EngineType = engine,
            EngineVolume = volume,
            Transmission = transmission,
            DriveType = drive,
            Price = price,
            Condition = condition,
            Description = description,
            Status = CarStatus.InStock
        };
}