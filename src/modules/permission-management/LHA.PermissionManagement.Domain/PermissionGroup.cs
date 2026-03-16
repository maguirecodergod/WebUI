using LHA.Ddd.Domain;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain;

/// <summary>
/// Layer 2 — A named group of permissions (e.g. "Tenant Management", "Store Management").
/// Groups permissions that logically belong together for easier assignment.
/// </summary>
public sealed class PermissionGroup : FullAuditedAggregateRoot<Guid>
{
    private readonly List<PermissionGroupItem> _items = [];

    /// <summary>Unique group name.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>Service that owns this group.</summary>
    public string ServiceName { get; private set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; private set; }

    /// <summary>Permissions in this group.</summary>
    public IReadOnlyCollection<PermissionGroupItem> Items => _items.AsReadOnly();

    private PermissionGroup() { }

    public PermissionGroup(Guid id, string name, string displayName, string serviceName, string? description = null)
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

    public PermissionGroup SetDisplayName(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
        return this;
    }

    public PermissionGroup SetDescription(string? description)
    {
        Description = description;
        return this;
    }

    public PermissionGroup AddPermission(Guid permissionId)
    {
        if (_items.Any(i => i.PermissionDefinitionId == permissionId)) return this;
        _items.Add(new PermissionGroupItem(Guid.CreateVersion7(), Id, permissionId));
        return this;
    }

    public PermissionGroup RemovePermission(Guid permissionId)
    {
        var existing = _items.FirstOrDefault(i => i.PermissionDefinitionId == permissionId);
        if (existing is not null) _items.Remove(existing);
        return this;
    }

    public PermissionGroup SyncPermissions(IEnumerable<Guid> permissionIds)
    {
        _items.Clear();
        foreach (var pid in permissionIds)
            _items.Add(new PermissionGroupItem(Guid.CreateVersion7(), Id, pid));
        return this;
    }
}
