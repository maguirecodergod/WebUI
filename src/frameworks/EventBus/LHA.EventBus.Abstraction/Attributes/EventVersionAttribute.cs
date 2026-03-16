namespace LHA.EventBus;

/// <summary>
/// Declares the schema version of an integration event for backward-compatible
/// event versioning. Used together with <see cref="IEventUpgrader{TSource, TTarget}"/>
/// for zero-downtime migration and Blue/Green deployments.
/// </summary>
/// <example>
/// <code>
/// [EventVersion(2)]
/// [EventName("OrderService.OrderPlaced")]
/// public sealed record OrderPlacedV2 : IntegrationEvent { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventVersionAttribute(int version) : Attribute
{
    /// <summary>The schema version (must be ≥ 1).</summary>
    public int Version { get; } = version >= 1
        ? version
        : throw new ArgumentOutOfRangeException(nameof(version), "Version must be ≥ 1.");
}
