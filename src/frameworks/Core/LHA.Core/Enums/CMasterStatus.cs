namespace LHA.Core;

/// <summary>
/// Master status enum used across all entities to indicate lifecycle state.
/// </summary>
public enum CMasterStatus
{
    /// <summary>
    /// 1 - Active: The entity is active and operational.
    /// </summary>
    Active = 1,

    /// <summary>
    /// 2 - InActive: The entity has been deactivated and is non-operational.
    /// </summary>
    InActive = 2
}
