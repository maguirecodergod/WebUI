namespace LHA.Auditing;

/// <summary>
/// Tracks a single entity change (create/update/delete) within an <see cref="AuditLogEntry"/>.
/// </summary>
public sealed class EntityChangeEntry
{
    /// <summary>UTC time the change occurred.</summary>
    public required DateTime ChangeTime { get; init; }

    /// <summary>Type of change.</summary>
    public required EntityChangeType ChangeType { get; init; }

    /// <summary>
    /// Tenant identifier of the changed entity.
    /// Not necessarily the same as the audit log's tenant (cross-tenant operations).
    /// </summary>
    public Guid? EntityTenantId { get; init; }

    /// <summary>Serialized entity primary key.</summary>
    public string? EntityId { get; init; }

    /// <summary>CLR full type name of the entity.</summary>
    public string? EntityTypeFullName { get; init; }

    /// <summary>Individual property changes.</summary>
    public List<EntityPropertyChange> PropertyChanges { get; init; } = [];

    /// <summary>
    /// Merges another change entry (same entity) into this one,
    /// combining property changes and retaining the latest values.
    /// </summary>
    public void Merge(EntityChangeEntry other)
    {
        ArgumentNullException.ThrowIfNull(other);

        foreach (var change in other.PropertyChanges)
        {
            var existing = PropertyChanges.Find(p => p.PropertyName == change.PropertyName);
            if (existing is not null)
            {
                existing.NewValue = change.NewValue;
            }
            else
            {
                PropertyChanges.Add(change);
            }
        }
    }
}
