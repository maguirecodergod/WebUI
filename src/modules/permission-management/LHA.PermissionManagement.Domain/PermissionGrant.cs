using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain;

/// <summary>
/// Records a permission grant to a provider (e.g. Role "R", User "U").
/// <para>
/// When ProviderName="R" and ProviderKey="{roleId}", the grant says
/// "this role has this permission".
/// When ProviderName="U" and ProviderKey="{userId}", the grant says
/// "this user has this permission directly".
/// </para>
/// </summary>
public sealed class PermissionGrant : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }

    /// <summary>The permission definition name being granted.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Provider type: "R" for Role, "U" for User.</summary>
    public string ProviderName { get; private set; } = null!;

    /// <summary>Provider key: role ID or user ID as string.</summary>
    public string ProviderKey { get; private set; } = null!;

    private PermissionGrant() { }

    public PermissionGrant(Guid id, string name, string providerName, string providerKey, Guid? tenantId = null)
    {
        Id = id;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        Name = name.Trim();
        ProviderName = providerName.Trim();
        ProviderKey = providerKey.Trim();
        TenantId = tenantId;
    }
}
