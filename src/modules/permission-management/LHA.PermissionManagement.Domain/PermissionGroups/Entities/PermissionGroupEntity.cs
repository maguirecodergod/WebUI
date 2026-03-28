using LHA.Ddd.Domain;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain.PermissionGroups;

/// <summary>
/// Layer 2 — A named group of permissions (e.g. "Tenant Management", "Store Management").
/// Groups permissions that logically belong together for easier assignment.
/// </summary>
public sealed class PermissionGroupEntity : FullAuditedAggregateRoot<Guid>
{
    private readonly List<PermissionGroupItemEntity> _items = [];

    /// <summary>Unique group name.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>Service that owns this group.</summary>
    public string ServiceName { get; private set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; private set; }

    /// <summary>Permissions in this group.</summary>
    public IReadOnlyCollection<PermissionGroupItemEntity> Items => _items.AsReadOnly();

    private PermissionGroupEntity() { }

    public PermissionGroupEntity(Guid id, string name, string displayName, string serviceName, string? description = null)
    {
        Id = id;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > PermissionGroupConsts.MaxNameLength)
            throw new ArgumentException($"Group name max {PermissionGroupConsts.MaxNameLength} chars.");

        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        Name = name.Trim();
        DisplayName = displayName.Trim();
        ServiceName = serviceName.Trim();
        Description = description;
    }

    public PermissionGroupEntity SetDisplayName(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
        return this;
    }

    public PermissionGroupEntity SetDescription(string? description)
    {
        Description = description;
        return this;
    }

    public PermissionGroupEntity AddPermission(Guid permissionId)
    {
        if (_items.Any(i => i.PermissionDefinitionId == permissionId)) return this;
        _items.Add(new PermissionGroupItemEntity(Guid.CreateVersion7(), Id, permissionId));
        return this;
    }

    public PermissionGroupEntity RemovePermission(Guid permissionId)
    {
        var existing = _items.FirstOrDefault(i => i.PermissionDefinitionId == permissionId);
        if (existing is not null) _items.Remove(existing);
        return this;
    }

    public PermissionGroupEntity SyncPermissions(IEnumerable<Guid> permissionIds)
    {
        _items.Clear();
        foreach (var pid in permissionIds)
            _items.Add(new PermissionGroupItemEntity(Guid.CreateVersion7(), Id, pid));
        return this;
    }
}
