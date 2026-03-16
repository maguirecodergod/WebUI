using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Tracks a single entity change (create/update/delete) within an <see cref="AuditLogEntity"/>.
/// </summary>
public sealed class EntityChangeEntity : Entity<Guid>, IMultiTenant
{
    private readonly List<EntityPropertyChangeEntity> _propertyChanges = [];

    /// <summary>FK to the parent audit log.</summary>
    public Guid AuditLogId { get; private init; }

    /// <summary>Tenant identifier (denormalized for filtering).</summary>
    public Guid? TenantId { get; private init; }

    /// <summary>UTC time the change occurred.</summary>
    public DateTimeOffset ChangeTime { get; private init; }

    /// <summary>Type of change.</summary>
    public EntityChangeType ChangeType { get; private init; }

    /// <summary>Tenant identifier of the changed entity (may differ from log tenant).</summary>
    public Guid? EntityTenantId { get; private init; }

    /// <summary>Serialized entity primary key.</summary>
    public string? EntityId { get; private init; }

    /// <summary>CLR full type name of the entity.</summary>
    public string? EntityTypeFullName { get; private init; }

    /// <summary>Individual property changes.</summary>
    public IReadOnlyCollection<EntityPropertyChangeEntity> PropertyChanges
        => _propertyChanges.AsReadOnly();

    /// <summary>EF Core constructor.</summary>
    private EntityChangeEntity() { }

    public EntityChangeEntity(
        Guid auditLogId,
        Guid? tenantId,
        DateTimeOffset changeTime,
        EntityChangeType changeType,
        Guid? entityTenantId,
        string? entityId,
        string? entityTypeFullName)
    {
        Id = Guid.CreateVersion7();
        AuditLogId = auditLogId;
        TenantId = tenantId;
        ChangeTime = changeTime;
        ChangeType = changeType;
        EntityTenantId = entityTenantId;
        EntityId = entityId;
        EntityTypeFullName = entityTypeFullName;
    }

    /// <summary>Adds a property change to this entity change.</summary>
    public void AddPropertyChange(EntityPropertyChangeEntity propertyChange)
    {
        _propertyChanges.Add(propertyChange);
    }
}
