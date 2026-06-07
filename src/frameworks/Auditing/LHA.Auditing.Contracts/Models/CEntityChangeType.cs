namespace LHA.Auditing;

/// <summary>
/// Type of entity change tracked in audit logs.
/// </summary>
public enum CEntityChangeType : byte
{
    /// <summary>
    /// A new entity was created.
    /// </summary>
    Created = 0,

    /// <summary>
    /// An existing entity was modified.
    /// </summary>
    Updated = 1,

    /// <summary>
    /// An entity was deleted (or soft-deleted).
    /// </summary>
    Deleted = 2
}
