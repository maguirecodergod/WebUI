using LHA.DistributedLocking;
using LHA.MessageBroker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.RabbitMQ;

/// <summary>
/// RabbitMQ-aware outbox processor that reads pending outbox messages
/// and forwards them to RabbitMQ via <see cref="IMessagePublisher"/>.
/// </summary>
internal sealed class RabbitMqOutboxProcessor
{
    private readonly IOutboxStore _outboxStore;
    private readonly IMessagePublisher _publisher;
    private readonly IEventNameResolver _nameResolver;
    private readonly IDistributedLock _distributedLock;
    private readonly EventBusOptions _options;
    private readonly RabbitMqEventBusOptions _rmqOptions;
    private readonly ILogger<RabbitMqOutboxProcessor> _logger;

    public RabbitMqOutboxProcessor(
        IOutboxStore outboxStore,
        IMessagePublisher publisher,
        IEventNameResolver nameResolver,
        IDistributedLock distributedLock,
        IOptions<EventBusOptions> options,
        IOptions<RabbitMqEventBusOptions> rmqOptions,
        ILogger<RabbitMqOutboxProcessor>? logger = null)
    {
        _outboxStore = outboxStore;
        _publisher = publisher;
        _nameResolver = nameResolver;
        _distributedLock = distributedLock;
        _options = options.Value;
        _rmqOptions = rmqOptions.Value;
        _logger = logger ?? NullLogger<RabbitMqOutboxProcessor>.Instance;
    }

    public async Task ProcessBatchAsync(CancellationToken ct = default)
    {
        if (!_options.EnableOutbox) return;

        var lockName = $"rmq-outbox:{_options.ConsumerGroup}";
        await using var handle = await _distributedLock.AcquireAsync(lockName, _options.OutboxPollingInterval, ct);

        if (handle is null)
        {
            _logger.LogDebug("Could not acquire RabbitMQ outbox lock. Another instance is processing.");
            return;
        }

        var messages = await _outboxStore.GetPendingAsync(_options.OutboxBatchSize, ct);
        if (messages.Count == 0) return;

        _logger.LogDebug("Processing {Count} RabbitMQ outbox messages.", messages.Count);

        foreach (var message in messages)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await ForwardToRabbitMqAsync(message, ct);
                await _outboxStore.MarkProcessedAsync(message.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to forward outbox message {Id} ('{EventName}') to RabbitMQ.",
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

    private async Task ForwardToRabbitMqAsync(OutboxMessageInfo message, CancellationToken ct)
    {
        var exchange = ResolveExchange(message.EventName);
        var routingKey = ResolveRoutingKey(message.EventName);

        var envelope = new MessageEnvelope<byte[]>
        {
            MessageId = message.Id.ToString("N"),
            Payload = message.Payload,
            TenantId = message.Metadata?.TenantId?.ToString(),
            CorrelationId = message.Metadata?.CorrelationId?.ToString(),
            CausationId = message.Metadata?.CausationId?.ToString(),
            Source = message.Metadata?.Source,
            SchemaVersion = message.EventVersion.ToString(),
            RoutingKey = routingKey,
            Timestamp = message.CreatedAtUtc,
            Metadata = new Dictionary<string, string>
            {
                ["x-event-name"] = message.EventName,
                ["x-event-version"] = message.EventVersion.ToString(),
            }
        };

        await _publisher.PublishAsync(exchange, envelope, ct);

        _logger.LogDebug("Forwarded outbox message {Id} to RabbitMQ exchange '{Exchange}'.", message.Id, exchange);
    }

    private string ResolveExchange(string eventName)
    {
        if (_rmqOptions.EventExchangeMappings.TryGetValue(eventName, out var exchange))
            return exchange;
        return _rmqOptions.DefaultExchange ?? "lha.events";
    }

    private string ResolveRoutingKey(string eventName)
    {
        if (_rmqOptions.EventRoutingKeyMappings.TryGetValue(eventName, out var rk))
            return rk;
        return eventName.Replace('.', '-').ToLowerInvariant();
    }
}
