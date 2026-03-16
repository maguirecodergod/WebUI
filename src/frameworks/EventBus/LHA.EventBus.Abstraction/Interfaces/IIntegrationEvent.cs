namespace LHA.EventBus;

/// <summary>
/// Marker interface for integration events that cross service boundaries.
/// <para>
/// All integration events carry an <see cref="EventId"/>, a UTC timestamp,
/// and a version number for backward-compatible schema evolution.
/// </para>
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>Globally unique event identifier (V7 UUID recommended for time-ordering).</summary>
    Guid EventId { get; }

    /// <summary>UTC instant when the event occurred in the source service.</summary>
    DateTimeOffset OccurredAtUtc { get; }

    /// <summary>Schema version for backward-compatible event versioning.</summary>
    int Version { get; }
}
