namespace LHA.Auditing;

/// <summary>
/// Standard interface for automatic optimistic concurrency versioning.
/// The version value is incremented whenever the entity is changed.
/// </summary>
public interface IHasEntityVersion
{
    /// <summary>Monotonically increasing version number.</summary>
    int EntityVersion { get; }
}
