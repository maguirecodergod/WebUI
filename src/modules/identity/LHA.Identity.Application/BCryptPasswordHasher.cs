using LHA.Identity.Domain;

namespace LHA.Identity.Application;

/// <summary>
/// BCrypt-based password hasher using the Enhanced algorithm (SHA-512).
/// </summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, BCrypt.Net.HashType.SHA512, WorkFactor);
    }

    /// <inheritdoc />
    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, hashedPassword, BCrypt.Net.HashType.SHA512);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
