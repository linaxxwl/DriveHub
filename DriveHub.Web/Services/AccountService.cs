using DriveHub.Web.Data;
using DriveHub.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Web.Services;

/// <summary>Регистрация и проверка входа пользователей.</summary>
public class AccountService
{
    private readonly AppDbContext _db;

    /// <summary>Создаёт сервис.</summary>
    /// <param name="db">Контекст базы данных.</param>
    public AccountService(AppDbContext db) => _db = db;

    /// <summary>Регистрирует клиента. Возвращает null, если e-mail уже занят.</summary>
    /// <param name="fullName">ФИО.</param>
    /// <param name="email">E-mail.</param>
    /// <param name="phone">Телефон.</param>
    /// <param name="password">Пароль.</param>
    public async Task<User?> RegisterAsync(string fullName, string email, string phone, string password)
    {
        email = email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email)) return null;

        var user = new User
        {
            FullName = fullName.Trim(),
            Email = email,
            Phone = phone.Trim(),
            PasswordHash = PasswordHasher.Hash(password),
            RoleId = 1 // Клиент
        };
        _db.Users.Add(user);
        _db.ActionLogs.Add(new ActionLog { User = user, ActionType = "Регистрация", Description = email });
        await _db.SaveChangesAsync();
        return user;
    }

    /// <summary>Проверяет e-mail и пароль. Возвращает пользователя или null.</summary>
    /// <param name="email">E-mail.</param>
    /// <param name="password">Пароль.</param>
    public async Task<User?> LoginAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash))
            return null;
        return user;
    }
}