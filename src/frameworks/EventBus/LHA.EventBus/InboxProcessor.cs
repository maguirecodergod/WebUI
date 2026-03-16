using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus;

/// <summary>
/// Processes incoming events through the idempotent inbox, ensuring
/// exactly-once processing semantics over at-least-once transports.
/// <para>
/// Before dispatching to handlers, checks the <see cref="IInboxStore"/>
/// to determine if the event has already been processed by this consumer group.
/// </para>
/// </summary>
public sealed class InboxProcessor
{
    private readonly IInboxStore _inboxStore;
    private readonly EventBusOptions _options;
    private readonly ILogger<InboxProcessor> _logger;

    public InboxProcessor(
        IInboxStore inboxStore,
        IOptions<EventBusOptions> options,
        ILogger<InboxProcessor>? logger = null)
    {
        _inboxStore = inboxStore;
        _options = options.Value;
        _logger = logger ?? NullLogger<InboxProcessor>.Instance;
    }

    /// <summary>
    /// Returns <c>true</c> if the event should be processed (not yet in inbox).
    /// Records the event in the inbox if it's new.
    /// </summary>
    public async Task<bool> TryBeginProcessingAsync(EventMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (!_options.EnableInbox) return true;

        var alreadyProcessed = await _inboxStore.ExistsAsync(
            metadata.EventId, _options.ConsumerGroup, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogDebug("Event {EventId} ('{EventName}') already processed by consumer group '{Group}'. Skipping.",
                metadata.EventId, metadata.EventName, _options.ConsumerGroup);
            return false;
        }

        // Record that we're processing this event
        await _inboxStore.SaveAsync(new InboxMessageInfo
        {
            EventId = metadata.EventId,
            EventName = metadata.EventName,
            ConsumerGroup = _options.ConsumerGroup,
            ReceivedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);

        return true;
    }

    /// <summary>
    /// Marks the event as successfully processed in the inbox.
    /// </summary>
    public Task CompleteProcessingAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        if (!_options.EnableInbox) return Task.CompletedTask;

        return _inboxStore.MarkProcessedAsync(eventId, _options.ConsumerGroup, cancellationToken);
    }

    /// <summary>
    /// Purges old inbox entries beyond the retention period.
    /// </summary>
    public Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.EnableInbox) return Task.CompletedTask;

        return _inboxStore.PurgeAsync(_options.InboxRetentionPeriod, cancellationToken);
    }
}
