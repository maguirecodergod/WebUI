namespace LHA.MessageBroker;

/// <summary>
/// Broker-agnostic result of a publish operation.
/// Broker-specific data (partition, offset, delivery tag) is in <see cref="BrokerMetadata"/>.
/// </summary>
public sealed record PublishResult
{
    /// <summary>
    /// The resolved destination the message was published to.
    /// Kafka: topic name (after tenant routing). RabbitMQ: exchange name.
    /// </summary>
    public required string Destination { get; init; }

    /// <summary>Message ID assigned to the published message.</summary>
    public string? MessageId { get; init; }

    /// <summary>Timestamp assigned by the broker (if available).</summary>
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Broker-specific metadata. Use extension methods for type-safe access:
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> partition, offset (see KafkaPublishResultExtensions)</item>
    ///   <item><b>RabbitMQ:</b> exchange, routingKey (see RabbitMqPublishResultExtensions)</item>
    /// </list>
    /// </summary>
    public IReadOnlyDictionary<string, object> BrokerMetadata { get; init; } = new Dictionary<string, object>();
}
