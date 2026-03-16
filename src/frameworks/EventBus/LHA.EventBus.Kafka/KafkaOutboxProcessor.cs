using System.Text.Json;
using LHA.DistributedLocking;
using LHA.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.Kafka;

/// <summary>
/// Kafka-aware outbox processor that reads pending outbox messages
/// and forwards them to Kafka via <see cref="IMessagePublisher"/>.
/// </summary>
internal sealed class KafkaOutboxProcessor
{
    private readonly IOutboxStore _outboxStore;
    private readonly IMessagePublisher _publisher;
    private readonly IEventNameResolver _nameResolver;
    private readonly IDistributedLock _distributedLock;
    private readonly EventBusOptions _options;
    private readonly KafkaEventBusOptions _kafkaOptions;
    private readonly ILogger<KafkaOutboxProcessor> _logger;

    public KafkaOutboxProcessor(
        IOutboxStore outboxStore,
        IMessagePublisher publisher,
        IEventNameResolver nameResolver,
        IDistributedLock distributedLock,
        IOptions<EventBusOptions> options,
        IOptions<KafkaEventBusOptions> kafkaOptions,
        ILogger<KafkaOutboxProcessor>? logger = null)
    {
        _outboxStore = outboxStore;
        _publisher = publisher;
        _nameResolver = nameResolver;
        _distributedLock = distributedLock;
        _options = options.Value;
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger ?? NullLogger<KafkaOutboxProcessor>.Instance;
    }

    public async Task ProcessBatchAsync(CancellationToken ct = default)
    {
        if (!_options.EnableOutbox) return;

        var lockName = $"kafka-outbox:{_options.ConsumerGroup}";
        await using var handle = await _distributedLock.AcquireAsync(lockName, _options.OutboxPollingInterval, ct);

        if (handle is null)
        {
            _logger.LogDebug("Could not acquire Kafka outbox lock. Another instance is processing.");
            return;
        }

        var messages = await _outboxStore.GetPendingAsync(_options.OutboxBatchSize, ct);
        if (messages.Count == 0) return;

        _logger.LogDebug("Processing {Count} Kafka outbox messages.", messages.Count);

        foreach (var message in messages)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await ForwardToKafkaAsync(message, ct);
                await _outboxStore.MarkProcessedAsync(message.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to forward outbox message {Id} ('{EventName}') to Kafka.",
                    message.Id, message.EventName);

                if (message.RetryCount >= _options.OutboxMaxRetryCount)
                {
                    _logger.LogError("Outbox message {Id} exceeded max retries. Moving to dead letter.", message.Id);
                    await _outboxStore.MarkProcessedAsync(message.Id, ct);
                }
                else
                {
                    await _outboxStore.IncrementRetryAsync(message.Id, ct);
                }
            }
        }
    }

    private async Task ForwardToKafkaAsync(OutboxMessageInfo message, CancellationToken ct)
    {
        var topic = ResolveTopic(message.EventName);

        var envelope = new MessageEnvelope<byte[]>
        {
            MessageId = message.Id.ToString("N"),
            Payload = message.Payload,
            TenantId = message.Metadata?.TenantId?.ToString(),
            CorrelationId = message.Metadata?.CorrelationId?.ToString(),
            CausationId = message.Metadata?.CausationId?.ToString(),
            Source = message.Metadata?.Source,
            SchemaVersion = message.EventVersion.ToString(),
            PartitionKey = message.PartitionKey ?? message.EventName, // Default partition key to event name for ordering
            Timestamp = message.CreatedAtUtc,
            Metadata = new Dictionary<string, string>
            {
                ["x-event-name"] = message.EventName,
                ["x-event-version"] = message.EventVersion.ToString(),
            }
        };

        await _publisher.PublishAsync(topic, envelope, ct);

        _logger.LogDebug("Forwarded outbox message {Id} to Kafka topic '{Topic}'.", message.Id, topic);
    }

    private string ResolveTopic(string eventName)
    {
        if (_kafkaOptions.EventTopicMappings.TryGetValue(eventName, out var topic))
            return topic;
        return _kafkaOptions.DefaultTopic ?? eventName.Replace('.', '-').ToLowerInvariant();
    }
}
