using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;

namespace LHA.Identity.Domain;

/// <summary>
/// Persists a permission grant for a provider (role or user).
/// A permission is granted by setting a row; absence means denied.
/// </summary>
public sealed class IdentityPermissionGrant : Entity<Guid>, IMultiTenant
{
    /// <inheritdoc />
    public Guid? TenantId { get; private init; }

    /// <summary>Permission name (e.g., "Users.Create", "Roles.Delete").</summary>
    public string Name { get; private init; } = null!;

    /// <summary>
    /// Provider type: <see cref="PermissionGrantProviderName.Role"/> ("R")
    /// or <see cref="PermissionGrantProviderName.User"/> ("U").
    /// </summary>
    public string ProviderName { get; private init; } = null!;

    /// <summary>
    /// Provider key: role ID (for "R") or user ID (for "U"), as string.
    /// </summary>
    public string ProviderKey { get; private init; } = null!;

    /// <summary>
    /// <c>true</c> = grant (allow); <c>false</c> = deny.
    /// User-level deny overrides role-level allow.
    /// </summary>
    public bool IsGranted { get; private init; } = true;

    /// <summary>EF Core constructor.</summary>
    private IdentityPermissionGrant() { }

    /// <summary>Creates a new permission grant.</summary>
    public IdentityPermissionGrant(
        Guid id,
        string name,
        string providerName,
        string providerKey,
        Guid? tenantId = null,
        bool isGranted = true)
    {
        Id = id;
        TenantId = tenantId;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        Name = name;
        ProviderName = providerName;
        ProviderKey = providerKey;
        IsGranted = isGranted;
    }
}
