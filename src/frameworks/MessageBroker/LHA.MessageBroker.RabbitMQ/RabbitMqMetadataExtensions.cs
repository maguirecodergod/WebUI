namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Well-known metadata keys for RabbitMQ-specific data in <see cref="PublishResult.BrokerMetadata"/>
/// and <see cref="MessageContext{T}.BrokerMetadata"/>.
/// </summary>
public static class RabbitMqMetadataKeys
{
    public const string Exchange = "rabbitmq.exchange";
    public const string RoutingKey = "rabbitmq.routingKey";
    public const string DeliveryTag = "rabbitmq.deliveryTag";
    public const string Queue = "rabbitmq.queue";
    public const string Redelivered = "rabbitmq.redelivered";
}

/// <summary>
/// Extension methods for type-safe access to RabbitMQ-specific metadata
/// in <see cref="PublishResult"/>.
/// </summary>
public static class RabbitMqPublishResultExtensions
{
    /// <summary>Gets the RabbitMQ exchange name from the publish result.</summary>
    public static string GetExchange(this PublishResult result)
        => (string)result.BrokerMetadata[RabbitMqMetadataKeys.Exchange];

    /// <summary>Gets the RabbitMQ routing key from the publish result.</summary>
    public static string GetRoutingKey(this PublishResult result)
        => (string)result.BrokerMetadata[RabbitMqMetadataKeys.RoutingKey];
}

/// <summary>
/// Extension methods for type-safe access to RabbitMQ-specific metadata in <see cref="MessageContext{T}"/>.
/// </summary>
public static class RabbitMqMessageContextExtensions
{
    /// <summary>Gets the RabbitMQ exchange name from the message context.</summary>
    public static string GetExchange<T>(this MessageContext<T> context) where T : class
        => (string)context.BrokerMetadata[RabbitMqMetadataKeys.Exchange];

    /// <summary>Gets the RabbitMQ routing key from the message context.</summary>
    public static string GetRoutingKey<T>(this MessageContext<T> context) where T : class
        => (string)context.BrokerMetadata[RabbitMqMetadataKeys.RoutingKey];

    /// <summary>Gets the RabbitMQ delivery tag from the message context.</summary>
    public static ulong GetDeliveryTag<T>(this MessageContext<T> context) where T : class
        => (ulong)context.BrokerMetadata[RabbitMqMetadataKeys.DeliveryTag];

    /// <summary>Gets the RabbitMQ queue name from the message context.</summary>
    public static string GetQueue<T>(this MessageContext<T> context) where T : class
        => (string)context.BrokerMetadata[RabbitMqMetadataKeys.Queue];

    /// <summary>Gets whether the message was redelivered by RabbitMQ.</summary>
    public static bool IsRedelivered<T>(this MessageContext<T> context) where T : class
        => (bool)context.BrokerMetadata[RabbitMqMetadataKeys.Redelivered];
}
