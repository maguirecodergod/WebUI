using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.MessageBroker.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Kafka;

/// <summary>
/// <see cref="IAuditLogDispatcher"/> that publishes audit log batches to Kafka.
/// <para>
/// Uses the shared <see cref="KafkaConnectionFactory"/> for producer reuse.
/// Batches are published in parallel for maximum throughput.
/// </para>
/// </summary>
internal sealed class KafkaAuditLogDispatcher : IAuditLogDispatcher
{
    private readonly KafkaConnectionFactory _connectionFactory;
    private readonly KafkaAuditLogOptions _options;
    private readonly ILogger<KafkaAuditLogDispatcher> _logger;

    public KafkaAuditLogDispatcher(
        KafkaConnectionFactory connectionFactory,
        IOptions<KafkaAuditLogOptions> options,
        ILogger<KafkaAuditLogDispatcher> logger)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IReadOnlyList<AuditLogRecord> records, CancellationToken cancellationToken = default)
    {
        var producer = _connectionFactory.GetProducer();
        var tasks = new List<Task<DeliveryResult<string, byte[]>>>(records.Count);

        foreach (var record in records)
        {
            var key = _options.PartitionByTenant && record.TenantId is not null
                ? record.TenantId
                : record.Id;

            var payload = JsonSerializer.SerializeToUtf8Bytes(record, AuditLogJsonContext.Default.AuditLogRecord);

            var headers = new Headers
            {
                { "audit.service", Encoding.UTF8.GetBytes(record.ServiceName ?? "unknown") },
                { "audit.action-type", Encoding.UTF8.GetBytes(record.ActionType.ToString()) },
                { "audit.timestamp", Encoding.UTF8.GetBytes(record.Timestamp.ToString("O")) }
            };

            if (record.TraceId is not null)
                headers.Add("audit.trace-id", Encoding.UTF8.GetBytes(record.TraceId));

            if (record.TenantId is not null)
                headers.Add("audit.tenant-id", Encoding.UTF8.GetBytes(record.TenantId));

            var message = new Message<string, byte[]>
            {
                Key = key,
                Value = payload,
                Headers = headers
            };

            tasks.Add(producer.ProduceAsync(_options.TopicName, message, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);

        _logger.LogDebug(
            "Dispatched {Count} audit logs to Kafka topic {Topic}.",
            results.Length, _options.TopicName);
    }
}
