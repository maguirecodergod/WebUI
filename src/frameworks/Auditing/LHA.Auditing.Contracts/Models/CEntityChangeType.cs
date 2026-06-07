namespace LHA.Auditing;

/// <summary>
/// Type of entity change tracked in audit logs.
/// </summary>
public enum CEntityChangeType : byte
{
    /// <summary>
    /// 0 - Created: A new entity was created.
    /// </summary>
    Created = 0,

    /// <summary>
    /// 1 - Updated: An existing entity was modified.
    /// </summary>
    Updated = 1,

    /// <summary>
    /// 2 - Deleted: An entity was deleted (or soft-deleted).
    /// </summary>
    Deleted = 2
}
