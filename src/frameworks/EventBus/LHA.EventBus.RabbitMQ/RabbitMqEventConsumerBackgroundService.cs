using System.Text;
using System.Text.Json;
using LHA.MessageBroker;
using LHA.MessageBroker.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LHA.EventBus.RabbitMQ;

/// <summary>
/// RabbitMQ consumer background service that bridges the MessageBroker consumer
/// to the EventBus handler pipeline with inbox deduplication, event versioning,
/// and region filtering.
/// </summary>
internal sealed class RabbitMqEventConsumerBackgroundService(
    RabbitMqConnectionManager connectionManager,
    IServiceScopeFactory scopeFactory,
    IEventNameResolver nameResolver,
    IOptions<EventBusOptions> options,
    IOptions<RabbitMqEventBusOptions> rmqOptions,
    ILogger<RabbitMqEventConsumerBackgroundService> logger) : BackgroundService
{
    private readonly EventBusOptions _options = options.Value;
    private readonly RabbitMqEventBusOptions _rmqOptions = rmqOptions.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var exchange = _rmqOptions.DefaultExchange ?? "lha.events";
        var queue = _rmqOptions.DefaultQueue ?? $"{_options.ConsumerGroup}.events";

        logger.LogInformation("RabbitMQ event consumer starting: exchange={Exchange} queue={Queue}", exchange, queue);

        var channel = await connectionManager.CreateConsumerChannelAsync(stoppingToken);

        // Declare topology
        await channel.ExchangeDeclareAsync(exchange, _rmqOptions.ExchangeType, durable: true,
            autoDelete: false, cancellationToken: stoppingToken);

        var queueArgs = new Dictionary<string, object?>();

        if (_rmqOptions.EnableDeadLetterExchange)
        {
            var dlxExchange = _rmqOptions.DeadLetterExchange ?? $"{exchange}.dlx";
            await channel.ExchangeDeclareAsync(dlxExchange, "topic", durable: true,
                autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync($"{queue}.dlq", durable: true, exclusive: false,
                autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync($"{queue}.dlq", dlxExchange, "#",
                cancellationToken: stoppingToken);

            queueArgs["x-dead-letter-exchange"] = dlxExchange;
        }

        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false,
            autoDelete: false, arguments: queueArgs, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(queue, exchange, "#",
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await ProcessMessageAsync(ea, stoppingToken);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message from queue '{Queue}'.", queue);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);

        // Keep alive until shutdown
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        var headers = ea.BasicProperties?.Headers;

        var eventName = GetHeaderString(headers, "x-event-name");
        if (string.IsNullOrEmpty(eventName))
        {
            logger.LogWarning("Received message without x-event-name header. Skipping.");
            return;
        }

        var eventType = nameResolver.GetType(eventName);
        if (eventType is null)
        {
            logger.LogWarning("No handler registered for event '{EventName}'. Skipping.", eventName);
            return;
        }

        var @event = JsonSerializer.Deserialize(ea.Body.Span, eventType, JsonOptions);
        if (@event is null)
        {
            logger.LogWarning("Failed to deserialize event '{EventName}'. Skipping.", eventName);
            return;
        }

        var versionStr = GetHeaderString(headers, "x-event-version");
        _ = int.TryParse(versionStr, out var eventVersion);

        var metadata = new EventMetadata
        {
            EventName = eventName,
            EventVersion = eventVersion > 0 ? eventVersion : 1,
            EventId = Guid.TryParse(ea.BasicProperties?.MessageId, out var mid) ? mid : Guid.CreateVersion7(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Guid.TryParse(ea.BasicProperties?.CorrelationId, out var cid) ? cid : null,
            TenantId = ParseGuidHeader(headers, MessageHeaders.TenantId),
            CausationId = ParseGuidHeader(headers, MessageHeaders.CausationId),
            Source = GetHeaderString(headers, MessageHeaders.Source),
            Region = GetHeaderString(headers, "x-region"),
            ConsumerGroup = _options.ConsumerGroup
        };

        // Region filtering
        if (_options.EnforceRegionFiltering
            && !string.IsNullOrWhiteSpace(_options.CurrentRegion)
            && !string.IsNullOrWhiteSpace(metadata.Region)
            && !string.Equals(metadata.Region, _options.CurrentRegion, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogDebug("Skipping event '{EventName}' — region mismatch.", eventName);
            return;
        }

        using var scope = scopeFactory.CreateScope();

        // Inbox deduplication
        if (_options.EnableInbox)
        {
            var inboxProcessor = scope.ServiceProvider.GetRequiredService<InboxProcessor>();
            if (!await inboxProcessor.TryBeginProcessingAsync(metadata, ct))
                return;

            try
            {
                await DispatchToHandlersAsync(scope.ServiceProvider, @event, eventType, metadata, ct);
                await inboxProcessor.CompleteProcessingAsync(metadata.EventId, ct);
            }
            catch
            {
                throw;
            }
        }
        else
        {
            await DispatchToHandlersAsync(scope.ServiceProvider, @event, eventType, metadata, ct);
        }
    }

    private static async Task DispatchToHandlersAsync(
        IServiceProvider sp, object @event, Type eventType, EventMetadata metadata, CancellationToken ct)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = sp.GetServices(handlerType);

        var context = new EventContext
        {
            Metadata = metadata,
            ServiceProvider = sp,
            CancellationToken = ct
        };

        foreach (var handler in handlers)
        {
            if (handler is null) continue;
            var method = handlerType.GetMethod("HandleAsync")!;
            var task = (Task)method.Invoke(handler, [@event, context, ct])!;
            await task;
        }
    }

    private static string? GetHeaderString(IDictionary<string, object?>? headers, string key)
    {
        if (headers is null || !headers.TryGetValue(key, out var value))
            return null;

        return value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string str => str,
            _ => value?.ToString()
        };
    }

    private static Guid? ParseGuidHeader(IDictionary<string, object?>? headers, string key)
    {
        var value = GetHeaderString(headers, key);
        return Guid.TryParse(value, out var guid) ? guid : null;
    }
}
