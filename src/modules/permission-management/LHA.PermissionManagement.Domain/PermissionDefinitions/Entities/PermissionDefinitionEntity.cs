using LHA.Ddd.Domain;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain.PermissionDefinitions;

/// <summary>
/// Layer 1 — Atomic permission definition (e.g. "tenant.create", "store.view").
/// Registered by microservices at startup. Immutable after creation (name is the key).
/// </summary>
public sealed class PermissionDefinitionEntity : Entity<Guid>
{
    /// <summary>Unique permission name, e.g. "tenant.create".</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>Service that owns this permission, e.g. "identity", "account".</summary>
    public string ServiceName { get; private set; } = null!;

    /// <summary>Logical group within the service, e.g. "Tenant Management".</summary>
    public string? GroupName { get; private set; }

    /// <summary>Optional long description.</summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Which multi-tenancy side(s) this permission applies to.
    /// Host-only permissions (e.g. tenant CRUD) are NOT granted to tenant users.
    /// Defaults to <see cref="MultiTenancySides.Both"/>.
    /// </summary>
    public MultiTenancySides MultiTenancySide { get; private set; } = MultiTenancySides.Both;

    private PermissionDefinitionEntity() { }

    public PermissionDefinitionEntity(
        Guid id, string name, string displayName,
        string serviceName, string? groupName = null, string? description = null,
        MultiTenancySides multiTenancySide = MultiTenancySides.Both)
    {
        Id = id;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > PermissionDefinitionConsts.MaxNameLength)
            throw new ArgumentException($"Permission name max {PermissionDefinitionConsts.MaxNameLength} chars.");

        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        Name = name.Trim();
        DisplayName = displayName.Trim();
        ServiceName = serviceName.Trim();
        GroupName = groupName?.Trim();
        Description = description;
        MultiTenancySide = multiTenancySide;
    }

    public PermissionDefinitionEntity UpdateDisplayInfo(string displayName, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
        Description = description;
        return this;
    }

    public PermissionDefinitionEntity SetMultiTenancySide(MultiTenancySides side)
    {
        MultiTenancySide = side;
        return this;
    }
}
