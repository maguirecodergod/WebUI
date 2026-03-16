namespace LHA.UnitOfWork;

/// <summary>
/// Domain event record enqueued within a <see cref="IUnitOfWork"/> and published
/// when the UoW completes successfully.
/// </summary>
public sealed class UnitOfWorkEventRecord
{
    /// <summary>The CLR type of the event.</summary>
    public Type EventType { get; }

    /// <summary>The event payload.</summary>
    public object EventData { get; }

    /// <summary>
    /// Monotonically increasing order for deterministic event ordering within a single UoW.
    /// </summary>
    public long EventOrder { get; }

    /// <summary>
    /// Whether this event should be published through an outbox for guaranteed delivery.
    /// Default: <c>true</c>.
    /// </summary>
    public bool UseOutbox { get; }

    /// <summary>Extra properties for cross-cutting concerns (e.g. tenant ID, correlation ID).</summary>
    public Dictionary<string, object> Properties { get; } = [];

    public UnitOfWorkEventRecord(
        Type eventType,
        object eventData,
        long eventOrder,
        bool useOutbox = true)
    {
        ArgumentNullException.ThrowIfNull(eventType);
        ArgumentNullException.ThrowIfNull(eventData);

        EventType = eventType;
        EventData = eventData;
        EventOrder = eventOrder;
        UseOutbox = useOutbox;
    }

    /// <summary>Global event order generator (process-wide, thread-safe).</summary>
    private static long s_lastOrder;

    /// <summary>Generates the next monotonically increasing event order.</summary>
    public static long NextOrder() => Interlocked.Increment(ref s_lastOrder);
}
