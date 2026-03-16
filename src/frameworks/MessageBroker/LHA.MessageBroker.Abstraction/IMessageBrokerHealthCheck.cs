namespace LHA.MessageBroker;

/// <summary>
/// Health check abstraction for message broker connections.
/// Register implementations in DI and use with ASP.NET Core health checks.
/// </summary>
public interface IMessageBrokerHealthCheck
{
    /// <summary>
    /// Checks the health of the broker connection.
    /// </summary>
    Task<BrokerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>The broker type identifier (e.g. "Kafka", "RabbitMQ").</summary>
    string BrokerType { get; }
}

/// <summary>
/// Result of a broker health check.
/// </summary>
public sealed record BrokerHealthResult
{
    /// <summary>Whether the broker is reachable and healthy.</summary>
    public required bool IsHealthy { get; init; }

    /// <summary>Human-readable status description.</summary>
    public string? Description { get; init; }

    /// <summary>Additional diagnostic data.</summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();

    /// <summary>Exception if the check failed.</summary>
    public Exception? Exception { get; init; }

    public static BrokerHealthResult Healthy(string? description = null)
        => new() { IsHealthy = true, Description = description ?? "Broker is healthy" };

    public static BrokerHealthResult Unhealthy(string description, Exception? exception = null)
        => new() { IsHealthy = false, Description = description, Exception = exception };
}
