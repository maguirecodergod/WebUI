using System.Text.Json;

namespace LHA.Scheduling;

/// <summary>
/// Contextual information passed to a job during execution.
/// Contains job identity, tenant context, parameters, and cancellation support.
/// </summary>
public sealed class JobContext
{
    /// <summary>Unique execution ID for this specific job run.</summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// Logical job ID. For recurring jobs this is the recurring job ID;
    /// for one-off jobs this matches <see cref="ExecutionId"/>.
    /// </summary>
    public required string JobId { get; init; }

    /// <summary>Fully qualified .NET type name of the job implementation.</summary>
    public required string JobType { get; init; }

    /// <summary>Tenant ID for multi-tenant job isolation.</summary>
    public string? TenantId { get; init; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>User ID of the actor who enqueued/scheduled the job.</summary>
    public string? UserId { get; init; }

    /// <summary>Current retry attempt number (0 = first attempt).</summary>
    public int RetryAttempt { get; init; }

    /// <summary>UTC timestamp when the job execution started.</summary>
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Cancellation token signaled when the job should abort.</summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Serialized job parameters as a JSON string.
    /// Use <see cref="GetParameters{T}"/> for typed access.
    /// </summary>
    public string? SerializedParameters { get; init; }

    /// <summary>
    /// Additional metadata key-value pairs passed through from the scheduler.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Scheduler-specific metadata (e.g. Hangfire job ID, Quartz fire instance ID).
    /// </summary>
    public IReadOnlyDictionary<string, object> SchedulerMetadata { get; init; } = new Dictionary<string, object>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Deserializes the job parameters to the specified type.
    /// Returns null if <see cref="SerializedParameters"/> is null or empty.
    /// </summary>
    public T? GetParameters<T>() where T : class
    {
        if (string.IsNullOrEmpty(SerializedParameters))
            return null;

        return JsonSerializer.Deserialize<T>(SerializedParameters, JsonOptions);
    }
}
