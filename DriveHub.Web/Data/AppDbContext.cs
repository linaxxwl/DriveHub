using DriveHub.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Data;

/// <summary>Контекст базы данных приложения DriveHub.</summary>
public class AppDbContext : DbContext
{
    /// <summary>Создаёт контекст с заданными параметрами.</summary>
    /// <param name="options">Параметры подключения (строка подключения, провайдер).</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<ActionLog> ActionLogs => Set<ActionLog>();

    /// <summary>Настраивает ключи, связи, ограничения и начальные данные.</summary>
    /// <param name="modelBuilder">Построитель модели EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ключи, имена которых EF Core не определяет автоматически
        modelBuilder.Entity<ActionLog>().HasKey(a => a.LogId);
        modelBuilder.Entity<CarModel>().HasKey(m => m.ModelId);

        // Составной ключ избранного: клиент + автомобиль
        modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.ClientId, f.CarId });

        // Уникальные поля
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Car>().HasIndex(c => c.Vin).IsUnique();
        modelBuilder.Entity<Deal>().HasIndex(d => d.ContractNumber).IsUnique();

        // Точность денежных полей
        modelBuilder.Entity<Car>().Property(c => c.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Car>().Property(c => c.EngineVolume).HasPrecision(4, 1);
        modelBuilder.Entity<Deal>().Property(d => d.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Deal>().Property(d => d.Discount).HasPrecision(5, 2);

        // Заявка - сделка: одна заявка может иметь 1 сделку
        modelBuilder.Entity<Deal>()
            .HasOne(d => d.Request)
            .WithOne()
            .HasForeignKey<Deal>(d => d.RequestId);

        // Запрет каскадного удаления (иначе SQL Server ругается на циклические пути)
        foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Начальные роли
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, Name = "Клиент" },
            new Role { RoleId = 2, Name = "Менеджер" },
            new Role { RoleId = 3, Name = "Администратор" });
    }
}