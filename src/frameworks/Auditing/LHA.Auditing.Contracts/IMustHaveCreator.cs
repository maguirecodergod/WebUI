namespace LHA.Auditing;

/// <summary>
/// Standard interface for an entity that MUST have a creator.
/// Use for entities always created within an authenticated user context.
/// </summary>
public interface IMustHaveCreator
{
    /// <summary>Identifier of the user who created this entity (never <c>default</c>).</summary>
    Guid CreatorId { get; }
}
