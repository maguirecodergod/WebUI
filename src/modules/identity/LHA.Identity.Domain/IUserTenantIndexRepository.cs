using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository for <see cref="IdentityUserTenantIndex"/> operations.
/// Provides fast username/email → tenant lookup.
/// </summary>
public interface IUserTenantIndexRepository : IRepository<IdentityUserTenantIndex, Guid>
{
    /// <summary>
    /// Finds tenant ID by normalized username.
    /// Returns null if username not found.
    /// </summary>
    Task<Guid?> FindTenantIdByNormalizedUserNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds tenant ID by normalized email.
    /// Returns null if email not found.
    /// </summary>
    Task<Guid?> FindTenantIdByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds (UserId, TenantId) by normalized username or email.
    /// Returns null if not found.
    /// </summary>
    Task<(Guid UserId, Guid? TenantId)?> FindUserAndTenantAsync(
        string normalizedUserNameOrEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username or email already exists across all tenants.
    /// Returns true if exists, false otherwise.
    /// </summary>
    Task<bool> ExistsAsync(
        string normalizedUserName,
        string normalizedEmail,
        CancellationToken cancellationToken = default);
}
