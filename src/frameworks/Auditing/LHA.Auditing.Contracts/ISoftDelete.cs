namespace LHA.Auditing;

/// <summary>
/// Marks an entity as soft-deletable. Soft-deleted entities are
/// logically removed but physically retained in the data store.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indicates whether this entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; }
}
