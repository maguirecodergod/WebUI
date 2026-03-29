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

/// <summary>
/// Abstraction to fetch tenant information (name, id) from Tenant module.
/// </summary>
public interface IUserTenantLookupService
{
    Task<List<(Guid Id, string Name)>> GetTenantsAsync(List<Guid> tenantIds, CancellationToken ct = default);
}

/// <summary>
/// Abstraction to fetch all available permissions from Permission module.
/// </summary>
public interface IPermissionStore
{
    Task<List<string>> GetAllPermissionsAsync(CancellationToken ct = default);
}

/// <summary>
/// Bridge abstraction to call Tenant management operations from within Identity module
/// without direct coupling to TenantManagement.Domain.
/// </summary>
public interface ITenantManagerBridge
{
    /// <summary>
    /// Creates a new tenant and returns its unique identifier.
    /// </summary>
    Task<Guid> CreateTenantAsync(string name, int databaseStyle, CancellationToken ct = default);
}
