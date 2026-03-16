namespace LHA.EventBus;

/// <summary>
/// Configuration options for the event bus infrastructure.
/// </summary>
public sealed class EventBusOptions
{
    /// <summary>
    /// Consumer group name for inbox deduplication and parallel consumers.
    /// Each deployment/service instance should use a unique group.
    /// </summary>
    public string ConsumerGroup { get; set; } = "default";

    /// <summary>
    /// Application/service name stamped on outgoing events as <see cref="IntegrationEvent.Source"/>.
    /// </summary>
    public string? ApplicationName { get; set; }

    // ── Outbox ──────────────────────────────────────────────────

    /// <summary>Enables the transactional outbox pattern for reliable delivery.</summary>
    public bool EnableOutbox { get; set; }

    /// <summary>Maximum number of outbox messages processed per batch.</summary>
    public int OutboxBatchSize { get; set; } = 100;

    /// <summary>Polling interval for the outbox background processor.</summary>
    public TimeSpan OutboxPollingInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Maximum outbox delivery retries before moving to dead letter.</summary>
    public int OutboxMaxRetryCount { get; set; } = 3;

    // ── Inbox ──────────────────────────────────────────────────

    /// <summary>Enables the idempotent inbox for exactly-once processing.</summary>
    public bool EnableInbox { get; set; }

    /// <summary>How long to keep processed inbox entries before purging.</summary>
    public TimeSpan InboxRetentionPeriod { get; set; } = TimeSpan.FromDays(7);

    // ── Region ──────────────────────────────────────────────────

    /// <summary>
    /// Current deployment region code (e.g., "EU", "US", "APAC").
    /// When set, the event bus only processes events matching this region
    /// and stamps outgoing events with it for data residency compliance.
    /// </summary>
    public string? CurrentRegion { get; set; }

    /// <summary>
    /// When <c>true</c>, events with a <see cref="IntegrationEvent.Region"/>
    /// that does not match <see cref="CurrentRegion"/> are silently skipped.
    /// </summary>
    public bool EnforceRegionFiltering { get; set; }
}
