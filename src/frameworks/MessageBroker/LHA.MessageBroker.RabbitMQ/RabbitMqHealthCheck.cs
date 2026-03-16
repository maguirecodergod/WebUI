using Microsoft.Extensions.Logging;

namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Health check for RabbitMQ broker connectivity.
/// Verifies connection is open and responsive.
/// </summary>
public sealed class RabbitMqHealthCheck : IMessageBrokerHealthCheck
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly ILogger<RabbitMqHealthCheck> _logger;

    public string BrokerType => "RabbitMQ";

    public RabbitMqHealthCheck(
        RabbitMqConnectionManager connectionManager,
        ILogger<RabbitMqHealthCheck> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task<BrokerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionManager.GetConnectionAsync(cancellationToken);

            if (!connection.IsOpen)
            {
                return BrokerHealthResult.Unhealthy("RabbitMQ connection is not open");
            }

            return BrokerHealthResult.Healthy(
                $"Connected to RabbitMQ. Endpoint: {connection.Endpoint}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ health check failed");
            return BrokerHealthResult.Unhealthy(
                "Failed to connect to RabbitMQ", ex);
        }
    }
}
