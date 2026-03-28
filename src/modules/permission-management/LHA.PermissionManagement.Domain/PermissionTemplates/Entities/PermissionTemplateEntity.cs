using LHA.Ddd.Domain;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain.PermissionTemplates;

/// <summary>
/// Layer 3 — A permission template (e.g. "TenantAdmin", "StoreManager", "Cashier").
/// Templates are composed of permission groups to quickly assign bundles of permissions.
/// </summary>
public sealed class PermissionTemplateEntity : FullAuditedAggregateRoot<Guid>
{
    private readonly List<PermissionTemplateItemEntity> _items = [];

    /// <summary>Unique template name.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>Optional description.</summary>
    public string? Description { get; private set; }

    /// <summary>Permission groups in this template.</summary>
    public IReadOnlyCollection<PermissionTemplateItemEntity> Items => _items.AsReadOnly();

    private PermissionTemplateEntity() { }

    public PermissionTemplateEntity(Guid id, string name, string displayName, string? description = null)
    {
        Id = id;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > PermissionTemplateConsts.MaxNameLength)
            throw new ArgumentException($"Template name max {PermissionTemplateConsts.MaxNameLength} chars.");

        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Name = name.Trim();
        DisplayName = displayName.Trim();
        Description = description;
    }

    public PermissionTemplateEntity SetDisplayName(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
        return this;
    }

    public PermissionTemplateEntity SetDescription(string? description)
    {
        Description = description;
        return this;
    }

    public PermissionTemplateEntity AddGroup(Guid permissionGroupId)
    {
        if (_items.Any(i => i.PermissionGroupId == permissionGroupId)) return this;
        _items.Add(new PermissionTemplateItemEntity(Guid.CreateVersion7(), Id, permissionGroupId));
        return this;
    }

    public PermissionTemplateEntity RemoveGroup(Guid permissionGroupId)
    {
        var existing = _items.FirstOrDefault(i => i.PermissionGroupId == permissionGroupId);
        if (existing is not null) _items.Remove(existing);
        return this;
    }

    public PermissionTemplateEntity SyncGroups(IEnumerable<Guid> groupIds)
    {
        _items.Clear();
        foreach (var gid in groupIds)
            _items.Add(new PermissionTemplateItemEntity(Guid.CreateVersion7(), Id, gid));
        return this;
    }
}
