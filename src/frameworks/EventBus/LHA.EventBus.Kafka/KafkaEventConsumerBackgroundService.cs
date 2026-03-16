using System.Text;
using System.Text.Json;
using LHA.MessageBroker;
using LHA.MessageBroker.Kafka;
using LHA.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.Kafka;

/// <summary>
/// Kafka consumer background service that bridges the MessageBroker consumer
/// to the EventBus handler pipeline with inbox deduplication, event versioning,
/// and region filtering.
/// </summary>
internal sealed class KafkaEventConsumerBackgroundService(
    KafkaConnectionFactory connectionFactory,
    IServiceScopeFactory scopeFactory,
    IEventNameResolver nameResolver,
    IOptions<EventBusOptions> options,
    IOptions<KafkaEventBusOptions> kafkaOptions,
    ILogger<KafkaEventConsumerBackgroundService> logger,
    string topic) : BackgroundService
{
    private readonly EventBusOptions _options = options.Value;
    private readonly KafkaEventBusOptions _kafkaOptions = kafkaOptions.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var groupId = _kafkaOptions.ConsumerGroupId ?? _options.ConsumerGroup;

        logger.LogInformation("Kafka event consumer starting: topic={Topic} group={Group}", topic, groupId);

        var consumer = connectionFactory.CreateConsumer(groupId);

        consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result?.Message?.Value is null) continue;

                    await ProcessMessageAsync(result.Message.Headers, result.Message.Value, stoppingToken);

                    consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error consuming message from topic '{Topic}'.", topic);
                    // Brief backoff before retrying
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(
        Confluent.Kafka.Headers? headers, byte[] payload, CancellationToken ct)
    {
        // Extract event name from headers
        var eventName = GetHeaderValue(headers, "x-event-name");
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

        // Deserialize the event
        var @event = JsonSerializer.Deserialize(payload, eventType, JsonOptions);
        if (@event is null)
        {
            logger.LogWarning("Failed to deserialize event '{EventName}'. Skipping.", eventName);
            return;
        }

        // Build metadata
        var versionStr = GetHeaderValue(headers, "x-event-version");
        _ = int.TryParse(versionStr, out var eventVersion);

        var metadata = new EventMetadata
        {
            EventName = eventName,
            EventVersion = eventVersion > 0 ? eventVersion : 1,
            EventId = ParseGuidHeader(headers, MessageHeaders.CorrelationId.Replace("correlation", "message")) ?? Guid.CreateVersion7(),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = ParseGuidHeader(headers, MessageHeaders.CorrelationId),
            CausationId = ParseGuidHeader(headers, MessageHeaders.CausationId),
            TenantId = ParseGuidHeader(headers, MessageHeaders.TenantId),
            Source = GetHeaderValue(headers, MessageHeaders.Source),
            Region = GetHeaderValue(headers, "x-region"),
            ConsumerGroup = _options.ConsumerGroup
        };

        // Region filtering
        if (_options.EnforceRegionFiltering
            && !string.IsNullOrWhiteSpace(_options.CurrentRegion)
            && !string.IsNullOrWhiteSpace(metadata.Region)
            && !string.Equals(metadata.Region, _options.CurrentRegion, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogDebug("Skipping event '{EventName}' — region '{Region}' ≠ '{CurrentRegion}'.",
                eventName, metadata.Region, _options.CurrentRegion);
            return;
        }

        // Dispatch in a scoped context with an active Unit of Work
        using var scope = scopeFactory.CreateScope();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin();

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
                // Inbox entry remains unprocessed for retry
                throw;
            }
        }
        else
        {
            await DispatchToHandlersAsync(scope.ServiceProvider, @event, eventType, metadata, ct);
        }

        await uow.CompleteAsync(ct);
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

    private static string? GetHeaderValue(Confluent.Kafka.Headers? headers, string key)
    {
        if (headers is null) return null;
        try
        {
            var header = headers.GetLastBytes(key);
            return header is not null ? Encoding.UTF8.GetString(header) : null;
        }
        catch
        {
            return null;
        }
    }

    private static Guid? ParseGuidHeader(Confluent.Kafka.Headers? headers, string key)
    {
        var value = GetHeaderValue(headers, key);
        return Guid.TryParse(value, out var guid) ? guid : null;
    }
}
