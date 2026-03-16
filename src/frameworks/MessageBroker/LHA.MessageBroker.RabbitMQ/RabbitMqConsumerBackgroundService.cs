using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Background service that consumes messages from a RabbitMQ queue,
/// deserializes them, and dispatches to the appropriate <see cref="IMessageHandler{T}"/>.
/// Supports multi-tenant filtering, dead-letter exchange, and retry with exponential backoff.
/// Automatically reconnects on channel/connection failures.
/// </summary>
/// <typeparam name="T">The message payload type.</typeparam>
public sealed class RabbitMqConsumerBackgroundService<T> : BackgroundService where T : class
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly IMessageSerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _globalOptions;
    private readonly RabbitMqSubscriptionOptions _subscription;
    private readonly ILogger<RabbitMqConsumerBackgroundService<T>> _logger;

    public RabbitMqConsumerBackgroundService(
        RabbitMqConnectionManager connectionManager,
        IMessageSerializer serializer,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> globalOptions,
        RabbitMqSubscriptionOptions subscription,
        ILogger<RabbitMqConsumerBackgroundService<T>> logger)
    {
        _connectionManager = connectionManager;
        _serializer = serializer;
        _scopeFactory = scopeFactory;
        _globalOptions = globalOptions.Value;
        _subscription = subscription;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting RabbitMQ consumer for exchange [{Exchange}] queue [{Queue}] routing [{RoutingKey}] type [{MessageType}]",
            _subscription.Exchange, _subscription.Queue, _subscription.RoutingKey, typeof(T).Name);

        // Reconnect loop: survives channel/connection drops
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "RabbitMQ consumer for [{Queue}] shutting down", _subscription.Queue);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RabbitMQ consumer for [{Queue}] failed, reconnecting in 5s...",
                    _subscription.Queue);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        var channel = await _connectionManager.CreateConsumerChannelAsync(stoppingToken);

        try
        {
            // Declare topology
            await DeclareTopologyAsync(channel, stoppingToken);

            // Set QoS
            var prefetch = _subscription.PrefetchCount ?? _globalOptions.Consumer.PrefetchCount;
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: prefetch,
                global: false,
                cancellationToken: stoppingToken);

            // Create async consumer
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, args) =>
            {
                await ProcessMessageAsync(channel, args, stoppingToken);
            };

            await channel.BasicConsumeAsync(
                queue: _subscription.Queue,
                autoAck: _globalOptions.Consumer.AutoAck,
                consumerTag: $"lha-{typeof(T).Name}-{Environment.MachineName}",
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "RabbitMQ consumer started on queue [{Queue}]", _subscription.Queue);

            // Wait until cancellation — the consumer runs via events
            var tcs = new TaskCompletionSource();
            await using var registration = stoppingToken.Register(() => tcs.TrySetResult());
            await tcs.Task;
        }
        finally
        {
            if (channel.IsOpen)
                await channel.CloseAsync(cancellationToken: CancellationToken.None);
            channel.Dispose();
            _logger.LogInformation("RabbitMQ consumer channel for [{Queue}] disposed", _subscription.Queue);
        }
    }

    private async Task DeclareTopologyAsync(IChannel channel, CancellationToken stoppingToken)
    {
        // Declare exchange
        await channel.ExchangeDeclareAsync(
            exchange: _subscription.Exchange,
            type: _subscription.ExchangeType,
            durable: _subscription.DurableExchange,
            autoDelete: _subscription.AutoDeleteExchange,
            arguments: null,
            cancellationToken: stoppingToken);

        // Declare queue with optional DLX
        var queueArgs = new Dictionary<string, object?>();
        if (!string.IsNullOrEmpty(_subscription.DeadLetterExchange))
        {
            queueArgs["x-dead-letter-exchange"] = _subscription.DeadLetterExchange;
        }

        await channel.QueueDeclareAsync(
            queue: _subscription.Queue,
            durable: _subscription.DurableQueue,
            exclusive: _subscription.ExclusiveQueue,
            autoDelete: _subscription.AutoDeleteQueue,
            arguments: queueArgs,
            cancellationToken: stoppingToken);

        // Bind queue to exchange
        await channel.QueueBindAsync(
            queue: _subscription.Queue,
            exchange: _subscription.Exchange,
            routingKey: _subscription.RoutingKey,
            arguments: null,
            cancellationToken: stoppingToken);

        // Declare DLX exchange if configured
        if (!string.IsNullOrEmpty(_subscription.DeadLetterExchange))
        {
            await channel.ExchangeDeclareAsync(
                exchange: _subscription.DeadLetterExchange,
                type: "topic",
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var dlqName = $"{_subscription.Queue}.dlq";
            await channel.QueueDeclareAsync(
                queue: dlqName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue: dlqName,
                exchange: _subscription.DeadLetterExchange,
                routingKey: "#",
                arguments: null,
                cancellationToken: stoppingToken);
        }

        _logger.LogDebug(
            "Topology declared: exchange [{Exchange}] → queue [{Queue}] via [{RoutingKey}]",
            _subscription.Exchange, _subscription.Queue, _subscription.RoutingKey);
    }

    private async Task ProcessMessageAsync(
        IChannel channel,
        BasicDeliverEventArgs args,
        CancellationToken stoppingToken)
    {
        var headers = ExtractHeaders(args.BasicProperties.Headers);

        // Tenant filtering
        if (!string.IsNullOrEmpty(_subscription.TenantIdFilter))
        {
            var messageTenantId = headers.GetValueOrDefault(MessageHeaders.TenantId);
            if (!string.Equals(messageTenantId, _subscription.TenantIdFilter, StringComparison.OrdinalIgnoreCase))
            {
                // Not for this tenant — ack and skip
                if (!_globalOptions.Consumer.AutoAck)
                    await channel.BasicAckAsync(args.DeliveryTag, multiple: false, stoppingToken);
                return;
            }
        }

        var context = BuildContext(args, headers);

        _logger.LogDebug(
            "Processing message from [{Exchange}] / [{RoutingKey}] queue [{Queue}] | TenantId={TenantId}",
            args.Exchange, args.RoutingKey, _subscription.Queue, context.TenantId);

        var success = false;
        var maxRetries = _subscription.MaxRetryAttempts;

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<T>>();

                success = await handler.HandleAsync(
                    context with { RetryAttempt = attempt },
                    stoppingToken);

                if (success) break;

                _logger.LogWarning(
                    "Handler returned false for [{Queue}] deliveryTag={DeliveryTag} attempt {Attempt}/{MaxRetries}",
                    _subscription.Queue, args.DeliveryTag,
                    attempt + 1, maxRetries + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Handler exception for [{Queue}] deliveryTag={DeliveryTag} attempt {Attempt}/{MaxRetries}",
                    _subscription.Queue, args.DeliveryTag,
                    attempt + 1, maxRetries + 1);
            }

            if (attempt < maxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(
                    _subscription.RetryBackoffBaseMs * Math.Pow(2, attempt));
                await Task.Delay(delay, stoppingToken);
            }
        }

        if (_globalOptions.Consumer.AutoAck) return;

        if (success)
        {
            await channel.BasicAckAsync(args.DeliveryTag, multiple: false, stoppingToken);
        }
        else
        {
            // Nack without requeue — message goes to DLX if configured
            await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, stoppingToken);

            _logger.LogWarning(
                "Message nacked from [{Queue}] deliveryTag={DeliveryTag} (sent to DLX: {HasDlx})",
                _subscription.Queue, args.DeliveryTag,
                !string.IsNullOrEmpty(_subscription.DeadLetterExchange));
        }
    }

    private MessageContext<T> BuildContext(
        BasicDeliverEventArgs args,
        IReadOnlyDictionary<string, string> headers)
    {
        var payload = _serializer.Deserialize<T>(args.Body.ToArray())
            ?? throw new InvalidOperationException(
                $"Failed to deserialize message from queue [{_subscription.Queue}] deliveryTag={args.DeliveryTag}");

        return new MessageContext<T>
        {
            Payload = payload,
            TenantId = headers.GetValueOrDefault(MessageHeaders.TenantId),
            CorrelationId = args.BasicProperties.CorrelationId
                ?? headers.GetValueOrDefault(MessageHeaders.CorrelationId),
            CausationId = headers.GetValueOrDefault(MessageHeaders.CausationId),
            UserId = headers.GetValueOrDefault(MessageHeaders.UserId),
            Source = headers.GetValueOrDefault(MessageHeaders.Source),
            SchemaVersion = headers.GetValueOrDefault(MessageHeaders.SchemaVersion),
            Destination = _subscription.Queue,
            Timestamp = args.BasicProperties.Timestamp.UnixTime > 0
                ? DateTimeOffset.FromUnixTimeSeconds(args.BasicProperties.Timestamp.UnixTime)
                : DateTimeOffset.UtcNow,
            Headers = headers,
            BrokerMetadata = new Dictionary<string, object>
            {
                [RabbitMqMetadataKeys.Exchange] = args.Exchange,
                [RabbitMqMetadataKeys.RoutingKey] = args.RoutingKey,
                [RabbitMqMetadataKeys.DeliveryTag] = args.DeliveryTag,
                [RabbitMqMetadataKeys.Queue] = _subscription.Queue,
                [RabbitMqMetadataKeys.Redelivered] = args.Redelivered
            }
        };
    }

    private static Dictionary<string, string> ExtractHeaders(IDictionary<string, object?>? headers)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (headers is null) return result;

        foreach (var kvp in headers)
        {
            if (kvp.Value is byte[] bytes)
            {
                result[kvp.Key] = Encoding.UTF8.GetString(bytes);
            }
            else if (kvp.Value is not null)
            {
                result[kvp.Key] = kvp.Value.ToString() ?? string.Empty;
            }
        }
        return result;
    }
}
