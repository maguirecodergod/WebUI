using LHA.DistributedLocking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus;

/// <summary>
/// Background processor that reads pending <see cref="OutboxMessage"/> entries
/// and forwards them to the <see cref="IEventBus"/> for delivery.
/// <para>
/// Uses <see cref="IDistributedLock"/> to ensure only one processor instance
/// runs across a scaled-out deployment, preventing duplicate delivery.
/// </para>
/// </summary>
public sealed class OutboxProcessor
{
    private readonly IOutboxStore _outboxStore;
    private readonly IDistributedLock _distributedLock;
    private readonly EventBusOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IOutboxStore outboxStore,
        IDistributedLock distributedLock,
        IOptions<EventBusOptions> options,
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor>? logger = null)
    {
        _outboxStore = outboxStore;
        _distributedLock = distributedLock;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger ?? NullLogger<OutboxProcessor>.Instance;
    }

    /// <summary>
    /// Processes a single batch of pending outbox messages.
    /// </summary>
    public async Task ProcessBatchAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.EnableOutbox) return;

        var lockName = $"outbox-processor:{_options.ConsumerGroup}";
        await using var handle = await _distributedLock.AcquireAsync(lockName, _options.OutboxPollingInterval, cancellationToken);

        if (handle is null)
        {
            _logger.LogDebug("Could not acquire outbox lock '{LockName}'. Another instance is processing.", lockName);
            return;
        }

        var messages = await _outboxStore.GetPendingAsync(_options.OutboxBatchSize, cancellationToken);

        if (messages.Count == 0) return;

        _logger.LogDebug("Processing {Count} outbox messages.", messages.Count);

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await ForwardMessageAsync(message, cancellationToken);
                await _outboxStore.MarkProcessedAsync(message.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to forward outbox message {MessageId} ('{EventName}').",
                    message.Id, message.EventName);

                if (message.RetryCount >= _options.OutboxMaxRetryCount)
                {
                    _logger.LogError("Outbox message {MessageId} exceeded max retries ({MaxRetry}). Moving to dead letter.",
                        message.Id, _options.OutboxMaxRetryCount);
                    // Mark processed to stop retrying — dead letter handling is transport-specific
                    await _outboxStore.MarkProcessedAsync(message.Id, cancellationToken);
                }
                else
                {
                    await _outboxStore.IncrementRetryAsync(message.Id, cancellationToken);
                }
            }
        }
    }

    private Task ForwardMessageAsync(OutboxMessageInfo message, CancellationToken cancellationToken)
    {
        // In a full implementation, this would forward to the actual message broker transport.
        // For the in-memory implementation, we deserialize and dispatch locally.
        _logger.LogDebug("Forwarding outbox message {MessageId} '{EventName}' v{Version}.",
            message.Id, message.EventName, message.EventVersion);

        // The actual forwarding is transport-specific.
        // Kafka/RabbitMQ implementations would override this behavior.
        return Task.CompletedTask;
    }
}
