namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Configuration options for the LHA RabbitMQ MessageBroker.
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>RabbitMQ host name.</summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>RabbitMQ AMQP port.</summary>
    public int Port { get; set; } = 5672;

    /// <summary>Authentication user name.</summary>
    public string UserName { get; set; } = "guest";

    /// <summary>Authentication password.</summary>
    public string Password { get; set; } = "guest";

    /// <summary>Virtual host.</summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>Client-provided connection name for management UI identification.</summary>
    public string? ConnectionName { get; set; }

    /// <summary>Global default tenant strategy for exchange/routing resolution.</summary>
    public TenantTopicStrategy DefaultTenantStrategy { get; set; } = TenantTopicStrategy.SharedTopic;

    /// <summary>Prefix for tenant-specific exchanges (used with TenantPrefixTopic strategy).</summary>
    public string TenantExchangePrefix { get; set; } = "tenant";

    /// <summary>Publisher-specific options.</summary>
    public RabbitMqPublisherOptions Publisher { get; set; } = new();

    /// <summary>Consumer-specific options.</summary>
    public RabbitMqConsumerOptions Consumer { get; set; } = new();

    /// <summary>Max connection retry attempts before failing.</summary>
    public int MaxConnectionRetries { get; set; } = 5;

    /// <summary>Base delay between connection retries in milliseconds (exponential backoff).</summary>
    public int ConnectionRetryDelayMs { get; set; } = 1000;

    /// <summary>Enable automatic connection recovery on network failures.</summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>Interval for automatic recovery attempts in milliseconds.</summary>
    public int NetworkRecoveryIntervalMs { get; set; } = 5000;
}

/// <summary>
/// RabbitMQ publisher-specific configuration.
/// </summary>
public sealed class RabbitMqPublisherOptions
{
    /// <summary>Mark messages as persistent (survives broker restart).</summary>
    public bool PersistentDelivery { get; set; } = true;

    /// <summary>If true, broker returns unroutable messages via BasicReturn.</summary>
    public bool MandatoryPublish { get; set; } = false;

    /// <summary>Default exchange type for auto-declared exchanges.</summary>
    public string DefaultExchangeType { get; set; } = "topic";

    /// <summary>Whether auto-declared exchanges are durable.</summary>
    public bool DurableExchanges { get; set; } = true;
}

/// <summary>
/// RabbitMQ consumer-specific configuration.
/// </summary>
public sealed class RabbitMqConsumerOptions
{
    /// <summary>QoS prefetch count — max unacknowledged messages per consumer.</summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>Whether to automatically acknowledge messages (not recommended for production).</summary>
    public bool AutoAck { get; set; } = false;
}

/// <summary>
/// Configuration for a specific RabbitMQ consumer subscription.
/// Defines the exchange, queue, and binding used by a background consumer.
/// </summary>
public sealed class RabbitMqSubscriptionOptions
{
    /// <summary>Exchange name to consume from.</summary>
    public required string Exchange { get; set; }

    /// <summary>Queue name. If empty, a server-generated name will be used.</summary>
    public required string Queue { get; set; }

    /// <summary>Routing key pattern for binding (supports wildcards with topic exchanges).</summary>
    public string RoutingKey { get; set; } = "#";

    /// <summary>Exchange type: topic, direct, fanout, headers.</summary>
    public string ExchangeType { get; set; } = "topic";

    /// <summary>Whether the exchange survives broker restart.</summary>
    public bool DurableExchange { get; set; } = true;

    /// <summary>Whether the exchange is automatically deleted when no queues are bound.</summary>
    public bool AutoDeleteExchange { get; set; } = false;

    /// <summary>Whether the queue survives broker restart.</summary>
    public bool DurableQueue { get; set; } = true;

    /// <summary>Whether the queue is exclusive to this connection.</summary>
    public bool ExclusiveQueue { get; set; } = false;

    /// <summary>Whether the queue is automatically deleted when the last consumer disconnects.</summary>
    public bool AutoDeleteQueue { get; set; } = false;

    /// <summary>Dead letter exchange for failed messages. If set, enables DLX on the queue.</summary>
    public string? DeadLetterExchange { get; set; }

    /// <summary>Max retry attempts before sending to dead-letter exchange.</summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>Retry backoff base in milliseconds (exponential).</summary>
    public int RetryBackoffBaseMs { get; set; } = 1000;

    /// <summary>Override QoS prefetch count for this specific subscription.</summary>
    public ushort? PrefetchCount { get; set; }

    /// <summary>Optional tenant ID filter — only process messages for this tenant.</summary>
    public string? TenantIdFilter { get; set; }
}
