using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Tracks a single property change within an <see cref="EntityChangeEntity"/>.
/// </summary>
public sealed class EntityPropertyChangeEntity : Entity<Guid>, IMultiTenant
{
    /// <summary>FK to the parent entity change.</summary>
    public Guid EntityChangeId { get; private init; }

    /// <summary>Tenant identifier (denormalized for filtering).</summary>
    public Guid? TenantId { get; private init; }

    /// <summary>Name of the changed property.</summary>
    public string PropertyName { get; private init; } = null!;

    /// <summary>CLR full type name of the property.</summary>
    public string PropertyTypeFullName { get; private init; } = null!;

    /// <summary>Serialized original value (<c>null</c> for Created changes).</summary>
    public string? OriginalValue { get; private init; }

    /// <summary>Serialized new value (<c>null</c> for Deleted changes).</summary>
    public string? NewValue { get; private init; }

    /// <summary>EF Core constructor.</summary>
    private EntityPropertyChangeEntity() { }

    public EntityPropertyChangeEntity(
        Guid entityChangeId,
        Guid? tenantId,
        string propertyName,
        string propertyTypeFullName,
        string? originalValue,
        string? newValue)
    {
        Id = Guid.CreateVersion7();
        EntityChangeId = entityChangeId;
        TenantId = tenantId;
        PropertyName = propertyName;
        PropertyTypeFullName = propertyTypeFullName;
        OriginalValue = originalValue;
        NewValue = newValue;
    }
}
