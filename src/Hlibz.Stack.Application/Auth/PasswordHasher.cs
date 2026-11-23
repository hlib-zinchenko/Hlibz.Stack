using System.Globalization;
using System.Security.Cryptography;

namespace Hlibz.Stack.Application.Auth;

/// <summary>
/// Hashes passwords with PBKDF2 (SHA-256, a random salt), stored as
/// <c>v1.{iterations}.{salt}.{hash}</c>.
/// </summary>
internal sealed class PasswordHasher
{
    private const int Iterations = 210_000;
    private const int SaltBytes = 16;
    private const int HashBytes = 32;

    /// <summary>
    /// A hash of no one's password, verified for an unknown email address so that a login's
    /// timing doesn't tell which addresses have accounts.
    /// </summary>
    private readonly Lazy<string> _dummy;

    public PasswordHasher()
    {
        _dummy = new Lazy<string>(() => Hash(Guid.NewGuid().ToString()));
    }

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}");
    }

    public bool Verify(string? storedHash, string password)
    {
        string[] parts = (storedHash ?? _dummy.Value).Split('.');
        if (parts is not ["v1", string iterations, string salt, string hash])
        {
            return false;
        }

        byte[] expected = Convert.FromBase64String(hash);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromBase64String(salt),
            int.Parse(iterations, CultureInfo.InvariantCulture),
            HashAlgorithmName.SHA256,
            expected.Length);
        return storedHash is not null && CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
