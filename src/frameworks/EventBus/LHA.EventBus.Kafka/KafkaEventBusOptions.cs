namespace LHA.EventBus.Kafka;

/// <summary>
/// Kafka-specific options for the event bus transport layer.
/// </summary>
public sealed class KafkaEventBusOptions
{
    /// <summary>
    /// Default Kafka topic when no explicit mapping is found.
    /// If <c>null</c>, the event name is converted to a topic name (dots → dashes, lowercase).
    /// </summary>
    public string? DefaultTopic { get; set; }

    /// <summary>
    /// Explicit event name → Kafka topic mappings.
    /// Allows routing specific events to dedicated topics.
    /// </summary>
    /// <example>
    /// <code>
    /// EventTopicMappings["OrderService.OrderPlaced"] = "order-events";
    /// </code>
    /// </example>
    public Dictionary<string, string> EventTopicMappings { get; set; } = [];

    /// <summary>
    /// Consumer group ID for the Kafka event consumer.
    /// Defaults to <see cref="EventBusOptions.ConsumerGroup"/> if not set.
    /// </summary>
    public string? ConsumerGroupId { get; set; }
}
