namespace LHA.EventBus.RabbitMQ;

/// <summary>
/// RabbitMQ-specific options for the event bus transport layer.
/// </summary>
public sealed class RabbitMqEventBusOptions
{
    /// <summary>
    /// Default exchange for event publishing.
    /// Defaults to "lha.events" if not set.
    /// </summary>
    public string? DefaultExchange { get; set; }

    /// <summary>
    /// Default queue name for consuming events.
    /// Defaults to "{ConsumerGroup}.events" if not set.
    /// </summary>
    public string? DefaultQueue { get; set; }

    /// <summary>
    /// Explicit event name → RabbitMQ exchange mappings.
    /// Allows routing specific events to dedicated exchanges.
    /// </summary>
    public Dictionary<string, string> EventExchangeMappings { get; set; } = [];

    /// <summary>
    /// Explicit event name → routing key mappings.
    /// When not mapped, the event name is used (dots → dashes, lowercase).
    /// </summary>
    public Dictionary<string, string> EventRoutingKeyMappings { get; set; } = [];

    /// <summary>
    /// Exchange type for auto-declared exchanges: "topic", "direct", "fanout", "headers".
    /// Defaults to "topic" for flexible routing.
    /// </summary>
    public string ExchangeType { get; set; } = "topic";

    /// <summary>
    /// Whether to create a dead-letter exchange for failed event processing.
    /// </summary>
    public bool EnableDeadLetterExchange { get; set; } = true;

    /// <summary>
    /// Dead-letter exchange name. Defaults to "{DefaultExchange}.dlx".
    /// </summary>
    public string? DeadLetterExchange { get; set; }
}
