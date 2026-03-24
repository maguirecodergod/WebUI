namespace LHA.Auditing;

/// <summary>
/// Standard interface to add a last-modification timestamp to an entity.
/// </summary>
public interface IHasModificationTime
{
    /// <summary>UTC instant of the most recent modification (<c>null</c> if never modified).</summary>
    DateTimeOffset? LastModificationTime { get; }
}
