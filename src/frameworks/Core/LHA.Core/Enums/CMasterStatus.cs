namespace LHA.Core;

/// <summary>
/// Master status enum used across all entities to indicate lifecycle state.
/// </summary>
public enum CMasterStatus
{
    /// <summary>
    /// The entity is active and operational.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The entity has been deactivated and is non-operational.
    /// </summary>
    InActive = 2
}
