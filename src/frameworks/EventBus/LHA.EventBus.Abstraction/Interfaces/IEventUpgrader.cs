namespace LHA.EventBus;

/// <summary>
/// Non-generic base for event upgraders, enabling pipeline chaining.
/// </summary>
public interface IEventUpgrader
{
    /// <summary>Source event type this upgrader reads from.</summary>
    Type SourceType { get; }

    /// <summary>Target event type this upgrader produces.</summary>
    Type TargetType { get; }

    /// <summary>Upgrades a source event object to the target type.</summary>
    object Upgrade(object source);
}

/// <summary>
/// Upgrades an event from schema version <typeparamref name="TSource"/>
/// to <typeparamref name="TTarget"/>.
/// <para>
/// Upgraders form a chain: V1 → V2 → V3. When a consumer running V3 receives
/// a V1 event, the pipeline applies V1→V2 then V2→V3 automatically.
/// This enables zero-downtime migration and backward-compatible event versioning.
/// </para>
/// </summary>
/// <typeparam name="TSource">The older event schema type.</typeparam>
/// <typeparam name="TTarget">The newer event schema type.</typeparam>
public interface IEventUpgrader<in TSource, out TTarget> : IEventUpgrader
    where TSource : class
    where TTarget : class
{
    Type IEventUpgrader.SourceType => typeof(TSource);
    Type IEventUpgrader.TargetType => typeof(TTarget);
    object IEventUpgrader.Upgrade(object source) => Upgrade((TSource)source);

    /// <summary>
    /// Transforms a <typeparamref name="TSource"/> event to <typeparamref name="TTarget"/>.
    /// </summary>
    TTarget Upgrade(TSource source);
}
