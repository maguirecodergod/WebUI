namespace LHA.Identity.Domain;

/// <summary>
/// Hashes and verifies passwords.
/// Implementation is registered via DI (e.g., BCrypt, PBKDF2).
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes a plain-text password.</summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    /// <returns><c>true</c> if the password matches the hash.</returns>
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}

/// <summary>
/// Normalizes strings for case-insensitive lookups
/// (user names, emails, role names).
/// </summary>
public interface ILookupNormalizer
{
    /// <summary>Normalizes a name for lookup (e.g., upper-case invariant).</summary>
    string NormalizeName(string name);

    /// <summary>Normalizes an email for lookup.</summary>
    string NormalizeEmail(string email);
}
