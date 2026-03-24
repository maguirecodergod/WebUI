namespace LHA.Auditing;

/// <summary>
/// Standard interface to add a deletion timestamp to a soft-deletable entity.
/// Extends <see cref="ISoftDelete"/> — implementing this implies soft-delete support.
/// </summary>
public interface IHasDeletionTime : ISoftDelete
{
    /// <summary>UTC instant when the entity was soft-deleted (<c>null</c> if not deleted).</summary>
    DateTimeOffset? DeletionTime { get; }
}
