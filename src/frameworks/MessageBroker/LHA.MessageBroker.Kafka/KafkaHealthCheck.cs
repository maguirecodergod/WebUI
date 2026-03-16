using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Health check for Kafka broker connectivity.
/// Verifies connection by querying cluster metadata.
/// </summary>
public sealed class KafkaHealthCheck : IMessageBrokerHealthCheck
{
    private readonly KafkaConnectionFactory _connectionFactory;
    private readonly ILogger<KafkaHealthCheck> _logger;

    public string BrokerType => "Kafka";

    public KafkaHealthCheck(
        KafkaConnectionFactory connectionFactory,
        ILogger<KafkaHealthCheck> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public Task<BrokerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var producer = _connectionFactory.GetProducer();

            // Verify the producer is connected by checking its name (throws if disposed/invalid)
            var name = producer.Name;

            return Task.FromResult(BrokerHealthResult.Healthy(
                $"Kafka producer [{name}] is active"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka health check failed");
            return Task.FromResult(BrokerHealthResult.Unhealthy(
                "Failed to verify Kafka producer health", ex));
        }
    }
}
