using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// RabbitMQ publisher implementing the broker-agnostic <see cref="IMessagePublisher"/> interface.
/// Routes messages to the correct exchange/routing key based on tenant strategy.
/// Thread-safe — uses a shared publish channel from <see cref="RabbitMqConnectionManager"/>.
/// </summary>
public sealed class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly IMessageSerializer _serializer;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessagePublisher> _logger;

    public RabbitMqMessagePublisher(
        RabbitMqConnectionManager connectionManager,
        IMessageSerializer serializer,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessagePublisher> logger)
    {
        _connectionManager = connectionManager;
        _serializer = serializer;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PublishResult> PublishAsync<T>(
        string destination,
        MessageEnvelope<T> envelope,
        CancellationToken cancellationToken = default) where T : class
    {
        var channel = await _connectionManager.GetPublishChannelAsync(cancellationToken);

        var resolvedExchange = ResolveExchange(destination, envelope.TenantId);
        var routingKey = ResolveRoutingKey(destination, envelope);

        // Auto-declare exchange (idempotent)
        await channel.ExchangeDeclareAsync(
            exchange: resolvedExchange,
            type: _options.Publisher.DefaultExchangeType,
            durable: _options.Publisher.DurableExchanges,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = envelope.Payload is byte[] raw
            ? raw
            : _serializer.Serialize(envelope.Payload);
        var properties = BuildBasicProperties(envelope);

        _logger.LogDebug(
            "Publishing to exchange [{Exchange}] routingKey [{RoutingKey}] | TenantId={TenantId} | MessageId={MessageId}",
            resolvedExchange, routingKey, envelope.TenantId, envelope.MessageId);

        await channel.BasicPublishAsync(
            exchange: resolvedExchange,
            routingKey: routingKey,
            mandatory: _options.Publisher.MandatoryPublish,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Published to [{Exchange}] / [{RoutingKey}] | TenantId={TenantId} | MessageId={MessageId}",
            resolvedExchange, routingKey, envelope.TenantId, envelope.MessageId);

        return new PublishResult
        {
            Destination = resolvedExchange,
            MessageId = envelope.MessageId,
            Timestamp = envelope.Timestamp,
            BrokerMetadata = new Dictionary<string, object>
            {
                [RabbitMqMetadataKeys.Exchange] = resolvedExchange,
                [RabbitMqMetadataKeys.RoutingKey] = routingKey
            }
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PublishResult>> PublishBatchAsync<T>(
        string destination,
        IEnumerable<MessageEnvelope<T>> envelopes,
        CancellationToken cancellationToken = default) where T : class
    {
        var channel = await _connectionManager.GetPublishChannelAsync(cancellationToken);
        var results = new List<PublishResult>();

        // Pre-declare exchange once for the batch
        var firstEnvelope = envelopes.FirstOrDefault();
        if (firstEnvelope is null) return results;

        var resolvedExchange = ResolveExchange(destination, firstEnvelope.TenantId);

        await channel.ExchangeDeclareAsync(
            exchange: resolvedExchange,
            type: _options.Publisher.DefaultExchangeType,
            durable: _options.Publisher.DurableExchanges,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        foreach (var envelope in envelopes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exchange = ResolveExchange(destination, envelope.TenantId);
            var routingKey = ResolveRoutingKey(destination, envelope);
            var body = envelope.Payload is byte[] rawBytes
                ? rawBytes
                : _serializer.Serialize(envelope.Payload);
            var properties = BuildBasicProperties(envelope);

            // Declare if tenant routing creates different exchanges
            if (exchange != resolvedExchange)
            {
                await channel.ExchangeDeclareAsync(
                    exchange: exchange,
                    type: _options.Publisher.DefaultExchangeType,
                    durable: _options.Publisher.DurableExchanges,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);
            }

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: _options.Publisher.MandatoryPublish,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            results.Add(new PublishResult
            {
                Destination = exchange,
                MessageId = envelope.MessageId,
                Timestamp = envelope.Timestamp,
                BrokerMetadata = new Dictionary<string, object>
                {
                    [RabbitMqMetadataKeys.Exchange] = exchange,
                    [RabbitMqMetadataKeys.RoutingKey] = routingKey
                }
            });
        }

        _logger.LogInformation(
            "Published batch of {Count} messages to exchange [{Exchange}]",
            results.Count, resolvedExchange);

        return results;
    }

    /// <summary>
    /// Resolves the actual exchange name based on tenant strategy.
    /// </summary>
    private string ResolveExchange(string baseExchange, string? tenantId)
    {
        return _options.DefaultTenantStrategy switch
        {
            TenantTopicStrategy.TenantPrefixTopic when !string.IsNullOrEmpty(tenantId)
                => $"{_options.TenantExchangePrefix}.{tenantId}.{baseExchange}",
            _ => baseExchange
        };
    }

    /// <summary>
    /// Resolves the routing key based on tenant strategy and envelope metadata.
    /// </summary>
    private string ResolveRoutingKey<T>(string baseExchange, MessageEnvelope<T> envelope) where T : class
    {
        var baseRoutingKey = envelope.RoutingKey ?? typeof(T).Name.ToLowerInvariant();

        return _options.DefaultTenantStrategy switch
        {
            TenantTopicStrategy.TenantPartition when !string.IsNullOrEmpty(envelope.TenantId)
                => $"{envelope.TenantId}.{baseRoutingKey}",
            _ => baseRoutingKey
        };
    }

    /// <summary>
    /// Builds AMQP basic properties with standard headers from the envelope.
    /// </summary>
    private BasicProperties BuildBasicProperties<T>(MessageEnvelope<T> envelope) where T : class
    {
        var headers = new Dictionary<string, object?>();

        AddHeader(headers, MessageHeaders.MessageType, typeof(T).AssemblyQualifiedName!);
        AddHeader(headers, MessageHeaders.SchemaVersion, envelope.SchemaVersion);
        AddHeader(headers, MessageHeaders.ContentType, _serializer.ContentType);

        if (!string.IsNullOrEmpty(envelope.TenantId))
            AddHeader(headers, MessageHeaders.TenantId, envelope.TenantId);
        if (!string.IsNullOrEmpty(envelope.CausationId))
            AddHeader(headers, MessageHeaders.CausationId, envelope.CausationId);
        if (!string.IsNullOrEmpty(envelope.UserId))
            AddHeader(headers, MessageHeaders.UserId, envelope.UserId);
        if (!string.IsNullOrEmpty(envelope.Source))
            AddHeader(headers, MessageHeaders.Source, envelope.Source);

        foreach (var kvp in envelope.Metadata)
            AddHeader(headers, kvp.Key, kvp.Value);

        return new BasicProperties
        {
            ContentType = _serializer.ContentType,
            DeliveryMode = _options.Publisher.PersistentDelivery
                ? DeliveryModes.Persistent
                : DeliveryModes.Transient,
            MessageId = envelope.MessageId,
            CorrelationId = envelope.CorrelationId ?? string.Empty,
            Timestamp = new AmqpTimestamp(envelope.Timestamp.ToUnixTimeSeconds()),
            Type = typeof(T).FullName,
            Headers = headers
        };
    }

    private static void AddHeader(Dictionary<string, object?> headers, string key, string value)
    {
        headers[key] = Encoding.UTF8.GetBytes(value);
    }
}
