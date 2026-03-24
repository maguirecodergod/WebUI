namespace LHA.Auditing;

/// <summary>
/// Standard interface for an entity that MAY have a creator.
/// Used for entities that can be created by the system without a user context.
/// </summary>
public interface IMayHaveCreator
{
    /// <summary>Identifier of the user who created this entity (<c>null</c> if system-created).</summary>
    Guid? CreatorId { get; }
}
