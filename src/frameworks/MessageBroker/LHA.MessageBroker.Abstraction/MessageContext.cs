namespace LHA.MessageBroker;

/// <summary>
/// Contextual information about a received message from any broker.
/// Broker-specific data (partition, offset, delivery tag, etc.) is accessible via <see cref="BrokerMetadata"/>.
/// </summary>
/// <typeparam name="T">The message payload type.</typeparam>
public sealed record MessageContext<T> where T : class
{
    /// <summary>The deserialized message payload.</summary>
    public required T Payload { get; init; }

    /// <summary>Tenant ID extracted from message headers.</summary>
    public string? TenantId { get; init; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>Causation ID linking to the originating command/event.</summary>
    public string? CausationId { get; init; }

    /// <summary>User ID of the actor.</summary>
    public string? UserId { get; init; }

    /// <summary>Source service/module that produced the message.</summary>
    public string? Source { get; init; }

    /// <summary>Schema version of the payload for compatibility checks.</summary>
    public string? SchemaVersion { get; init; }

    /// <summary>
    /// Logical destination the message was consumed from.
    /// Kafka: topic name. RabbitMQ: queue name.
    /// </summary>
    public required string Destination { get; init; }

    /// <summary>Message timestamp (from broker or envelope).</summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>All raw headers as string dictionary.</summary>
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

    /// <summary>Current retry attempt number (0 = first attempt).</summary>
    public int RetryAttempt { get; init; }

    /// <summary>
    /// Broker-specific metadata. Use extension methods for type-safe access:
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> topic, partition, offset (see KafkaMessageContextExtensions)</item>
    ///   <item><b>RabbitMQ:</b> exchange, routingKey, deliveryTag (see RabbitMqMessageContextExtensions)</item>
    /// </list>
    /// </summary>
    public IReadOnlyDictionary<string, object> BrokerMetadata { get; init; } = new Dictionary<string, object>();
}
