namespace LHA.Auditing;

/// <summary>
/// Standard interface to add a creation timestamp to an entity.
/// </summary>
public interface IHasCreationTime
{
    /// <summary>UTC instant when the entity was created.</summary>
    DateTimeOffset CreationTime { get; }
}
