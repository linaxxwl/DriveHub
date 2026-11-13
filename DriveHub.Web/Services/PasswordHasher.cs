using System.Security.Cryptography;

namespace DriveHub.Web.Services;

/// <summary>Хэширование и проверка паролей.</summary>
public static class PasswordHasher
{
    private const int SaltSize = 16, HashSize = 32, Iterations = 100_000;

    /// <summary>Возвращает хэш пароля.</summary>
    /// <param name="password">Пароль в открытом виде.</param>
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    /// <summary>Проверяет пароль по сохраненному хэшу.</summary>
    /// <param name="password">Введенный пароль.</param>
    /// <param name="stored">Хэш из базы.</param>
    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}