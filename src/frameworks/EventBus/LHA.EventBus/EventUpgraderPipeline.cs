namespace LHA.EventBus;

/// <summary>
/// Chains <see cref="IEventUpgrader"/> instances to upgrade events
/// from older schema versions to the current version.
/// <para>
/// Supports multi-hop upgrades (V1 → V2 → V3) for zero-downtime migration
/// and backward-compatible event versioning during Blue/Green deployments.
/// </para>
/// </summary>
public sealed class EventUpgraderPipeline
{
    private readonly IReadOnlyList<IEventUpgrader> _upgraders;

    public EventUpgraderPipeline(IEnumerable<IEventUpgrader> upgraders)
    {
        _upgraders = upgraders.ToList();
    }

    /// <summary>
    /// Upgrades the source object through the chain of upgraders until
    /// it reaches the <paramref name="targetType"/> or no further upgrade path exists.
    /// </summary>
    /// <param name="source">The event object to upgrade.</param>
    /// <param name="targetType">Desired final type.</param>
    /// <returns>The upgraded object, or the original if no upgrade path exists.</returns>
    public object Upgrade(object source, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(source);

        var current = source;
        var currentType = current.GetType();
        var visited = new HashSet<Type>();

        while (currentType != targetType && visited.Add(currentType))
        {
            var upgrader = _upgraders.FirstOrDefault(u => u.SourceType == currentType);
            if (upgrader is null) break;

            current = upgrader.Upgrade(current);
            currentType = current.GetType();
        }

        return current;
    }
}
