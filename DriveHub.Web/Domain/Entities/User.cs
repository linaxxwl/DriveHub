namespace DriveHub.Web.Domain.Entities;

// Пользователь системы
public class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Блокирует учетную запись
    public void Block() => IsActive = false;

    //Разблокирует учетную запись
    public void Unblock() => IsActive = true;
}