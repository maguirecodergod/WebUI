namespace LHA.MessageBroker;

/// <summary>
/// Unified message envelope that wraps any payload with tenant context,
/// correlation metadata, and routing information.
/// Designed for multi-tenant SaaS with millions of tenants.
/// Broker-agnostic — works with both Kafka and RabbitMQ.
/// </summary>
/// <typeparam name="TPayload">The message payload type.</typeparam>
public sealed class MessageEnvelope<TPayload> where TPayload : class
{
    /// <summary>Unique message identifier.</summary>
    public string MessageId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>Tenant identifier for multi-tenant isolation.</summary>
    public string? TenantId { get; init; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>Causation ID linking to the originating event/command.</summary>
    public string? CausationId { get; init; }

    /// <summary>User who triggered this message.</summary>
    public string? UserId { get; init; }

    /// <summary>Source service/module name.</summary>
    public string? Source { get; init; }

    /// <summary>Schema version for payload backward/forward compatibility.</summary>
    public string SchemaVersion { get; init; } = "1.0";

    /// <summary>UTC timestamp when the message was created.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>The message payload.</summary>
    public required TPayload Payload { get; init; }

    /// <summary>
    /// Partition key for Kafka (determines partition assignment via consistent hashing).
    /// Ignored by RabbitMQ.
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Routing key for RabbitMQ (used with topic/direct exchanges for message routing).
    /// If null, implementations should derive from message type name.
    /// Ignored by Kafka.
    /// </summary>
    public string? RoutingKey { get; init; }

    /// <summary>Additional custom metadata headers passed through to the broker.</summary>
    public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
