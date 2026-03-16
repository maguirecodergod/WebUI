namespace LHA.BackgroundJob.RabbitMQ;

/// <summary>
/// Options for the RabbitMQ-backed background job system.
/// </summary>
public sealed class RabbitMqBackgroundJobOptions
{
    /// <summary>
    /// Prefix for RabbitMQ queue names. Each job type gets its own queue:
    /// <c>{QueuePrefix}.{JobName}</c>.
    /// Default: <c>"lha.bg.jobs"</c>.
    /// </summary>
    public string QueuePrefix { get; set; } = "lha.bg.jobs";

    /// <summary>
    /// The exchange used for publishing background jobs.
    /// Default: <c>"lha.bg.exchange"</c> (direct exchange).
    /// </summary>
    public string ExchangeName { get; set; } = "lha.bg.exchange";

    /// <summary>
    /// Exchange type. Default: <c>"direct"</c> (routes by job name as routing key).
    /// </summary>
    public string ExchangeType { get; set; } = "direct";

    /// <summary>
    /// Whether queues and exchanges should be durable (survive broker restarts).
    /// Default: <see langword="true"/>.
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// QoS prefetch count per consumer channel.
    /// Default: <c>10</c>.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>
    /// Dead-letter exchange name. Failed jobs (after max retries) are sent here.
    /// Default: <c>"lha.bg.dlx"</c>.
    /// </summary>
    public string DeadLetterExchange { get; set; } = "lha.bg.dlx";

    /// <summary>
    /// Maximum delivery attempts before sending to the dead-letter exchange.
    /// Default: <c>3</c>.
    /// </summary>
    public int MaxDeliveryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay in milliseconds before reconnecting after a consumer channel failure.
    /// Default: <c>5000</c>.
    /// </summary>
    public int ReconnectDelayMs { get; set; } = 5000;
}
