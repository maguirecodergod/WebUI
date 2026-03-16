namespace LHA.MessageBroker;

/// <summary>
/// Standard header keys used in message headers for cross-cutting concerns.
/// These keys are consistent across all broker implementations (Kafka, RabbitMQ).
/// </summary>
public static class MessageHeaders
{
    /// <summary>Tenant identifier for multi-tenant routing and filtering.</summary>
    public const string TenantId = "x-tenant-id";

    /// <summary>Correlation ID for distributed tracing across services.</summary>
    public const string CorrelationId = "x-correlation-id";

    /// <summary>Causation ID linking to the event that caused this message.</summary>
    public const string CausationId = "x-causation-id";

    /// <summary>Fully qualified .NET type name of the message payload.</summary>
    public const string MessageType = "x-message-type";

    /// <summary>UTC timestamp when the message was created (ISO 8601).</summary>
    public const string Timestamp = "x-timestamp";

    /// <summary>User ID of the actor who triggered the message.</summary>
    public const string UserId = "x-user-id";

    /// <summary>Source service/module that produced the message.</summary>
    public const string Source = "x-source";

    /// <summary>Schema version for payload backward/forward compatibility.</summary>
    public const string SchemaVersion = "x-schema-version";

    /// <summary>Content type of the serialized payload (e.g. "application/json").</summary>
    public const string ContentType = "x-content-type";
}
