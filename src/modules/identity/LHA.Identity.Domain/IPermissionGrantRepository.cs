using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository abstraction for <see cref="IdentityPermissionGrant"/> persistence.
/// </summary>
public interface IPermissionGrantRepository : IRepository<IdentityPermissionGrant, Guid>
{
    /// <summary>
    /// Finds a specific permission grant.
    /// </summary>
    Task<IdentityPermissionGrant?> FindAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all permission grants for a specific provider (e.g., all permissions for a role).
    /// </summary>
    Task<List<IdentityPermissionGrant>> GetListAsync(
        string providerName, string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all permission grants matching the permission name.
    /// </summary>
    Task<List<IdentityPermissionGrant>> GetListByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific permission grant.
    /// </summary>
    Task DeleteAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all permission grants for the specified provider keys.
    /// Used to resolve permissions for multiple roles or a single user at once.
    /// </summary>
    Task<List<IdentityPermissionGrant>> GetGrantsByProvidersAsync(
        string providerName, IReadOnlyCollection<string> providerKeys,
        CancellationToken cancellationToken = default);
}
