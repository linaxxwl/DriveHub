using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;
using DriveHub.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Tests;

/// <summary>Тесты сервиса менеджера. База данных: SQLite в памяти (создается заново для каждого теста).</summary>
public class ManagerServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly ManagerService _service;
    private readonly int _managerId;
    private readonly int _client1Id;
    private readonly int _client2Id;
    private readonly int _carId;

    /// <summary>Создает пустую базу и добавляет менеджера, двух клиентов и один автомобиль в наличии.</summary>
    public ManagerServiceTests()
    {
        // Соединение держим открытым: база в памяти живет, пока оно не закрыто
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated(); // роли 1-3 добавляются из HasData

        var manager = NewUser(2, "manager@test.ru");
        var client1 = NewUser(1, "client1@test.ru");
        var client2 = NewUser(1, "client2@test.ru");
        var brand = new Brand { Name = "Kia", Country = "Южная Корея" };
        var model = new CarModel { Brand = brand, Name = "Rio", BodyType = "Седан", ProductionStartYear = 2011 };
        var car = new Car { Model = model, Vin = "VIN0001", Status = CarStatus.InStock, Price = 1_000_000m };
        _db.AddRange(manager, client1, client2, car);
        _db.SaveChanges();

        _managerId = manager.UserId;
        _client1Id = client1.UserId;
        _client2Id = client2.UserId;
        _carId = car.CarId;
        _service = new ManagerService(_db);
    }

    /// <summary>Освобождает базу и соединение.</summary>
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task Approve_PurchaseRequest_ReservesCar()
    {
        int requestId = AddRequest(_client1Id, RequestType.Purchase, RequestStatus.InProgress);

        await _service.ApproveAsync(requestId, _managerId);

        _db.ChangeTracker.Clear(); // читаем из базы, а не из памяти контекста
        Assert.Equal(RequestStatus.Approved, _db.Requests.Find(requestId)!.Status);
        Assert.Equal(CarStatus.Reserved, _db.Cars.Find(_carId)!.Status);
    }

    [Fact]
    public async Task Approve_TestDriveRequest_DoesNotReserveCar()
    {
        int requestId = AddRequest(_client1Id, RequestType.TestDrive, RequestStatus.InProgress);

        await _service.ApproveAsync(requestId, _managerId);

        _db.ChangeTracker.Clear();
        Assert.Equal(RequestStatus.Approved, _db.Requests.Find(requestId)!.Status);
        Assert.Equal(CarStatus.InStock, _db.Cars.Find(_carId)!.Status);
    }

    [Fact]
    public async Task Approve_PurchaseRequest_RejectsCompetingRequests()
    {
        int winnerId = AddRequest(_client1Id, RequestType.Purchase, RequestStatus.InProgress);
        int competitorId = AddRequest(_client2Id, RequestType.Purchase, RequestStatus.New);

        await _service.ApproveAsync(winnerId, _managerId);

        _db.ChangeTracker.Clear();
        var competitor = _db.Requests.Find(competitorId)!;
        Assert.Equal(RequestStatus.Rejected, competitor.Status);
        Assert.Contains("зарезервирован", competitor.ManagerComment);
    }

    [Fact]
    public async Task Approve_NewRequest_ThrowsAndCarStaysInStock()
    {
        int requestId = AddRequest(_client1Id, RequestType.Purchase, RequestStatus.New);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ApproveAsync(requestId, _managerId));

        _db.ChangeTracker.Clear();
        Assert.Equal(RequestStatus.New, _db.Requests.Find(requestId)!.Status);
        Assert.Equal(CarStatus.InStock, _db.Cars.Find(_carId)!.Status);
    }

    [Fact]
    public async Task Reject_WithoutComment_ThrowsAndStatusUnchanged()
    {
        int requestId = AddRequest(_client1Id, RequestType.Purchase, RequestStatus.InProgress);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.RejectAsync(requestId, _managerId, "  "));

        _db.ChangeTracker.Clear();
        Assert.Equal(RequestStatus.InProgress, _db.Requests.Find(requestId)!.Status);
    }

    /// <summary>Создает пользователя для тестовой базы.</summary>
    /// <param name="roleId">Роль (1 - клиент, 2 - менеджер).</param>
    /// <param name="email">E-mail (уникален).</param>
    private static User NewUser(int roleId, string email) =>
        new() { RoleId = roleId, FullName = email, Email = email, Phone = "+70000000000", PasswordHash = "x" };

    /// <summary>Добавляет заявку на тестовый автомобиль и возвращает ее номер.</summary>
    /// <param name="clientId">Клиент.</param>
    /// <param name="type">Тип заявки.</param>
    /// <param name="status">Начальный статус.</param>
    private int AddRequest(int clientId, RequestType type, RequestStatus status)
    {
        var request = new Request
        {
            ClientId = clientId,
            CarId = _carId,
            Type = type,
            Status = status,
            DesiredDate = DateTime.Today.AddDays(1)
        };
        _db.Requests.Add(request);
        _db.SaveChanges();
        return request.RequestId;
    }
}