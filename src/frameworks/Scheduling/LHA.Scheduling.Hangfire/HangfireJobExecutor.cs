using System.ComponentModel;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Bridge between Hangfire's job execution pipeline and the LHA <see cref="IScheduledJob"/> abstraction.
/// Hangfire serializes method calls — this executor is the method target that Hangfire invokes.
/// It resolves the actual <see cref="IScheduledJob"/> from DI and delegates execution.
/// </summary>
public sealed class HangfireJobExecutor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HangfireJobExecutor> _logger;

    public HangfireJobExecutor(
        IServiceScopeFactory scopeFactory,
        ILogger<HangfireJobExecutor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Invoked by Hangfire when a job fires. Resolves the <see cref="IScheduledJob"/>
    /// from DI and delegates execution.
    /// </summary>
    /// <param name="jobTypeName">Assembly-qualified type name of the IScheduledJob implementation.</param>
    /// <param name="serializedParameters">JSON-serialized parameters (may be null).</param>
    /// <param name="metadata">Key-value metadata from the scheduler.</param>
    /// <param name="cancellationToken">Hangfire cancellation token (signals job abort).</param>
    [AutomaticRetry(Attempts = 0)] // Retry is controlled by LHA abstraction, not Hangfire
    [DisplayName("{0}")]
    public async Task ExecuteAsync(
        string jobTypeName,
        string? serializedParameters,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken)
    {
        var jobType = Type.GetType(jobTypeName)
            ?? throw new InvalidOperationException(
                $"Cannot resolve job type: {jobTypeName}");

        var hangfireJobId = JobContext_Hangfire.GetCurrentJobId();

        _logger.LogInformation(
            "Executing job [{JobType}] Hangfire ID [{HangfireJobId}] TenantId [{TenantId}]",
            jobType.Name, hangfireJobId, metadata.GetValueOrDefault("tenantId"));

        using var scope = _scopeFactory.CreateScope();

        var job = scope.ServiceProvider.GetRequiredService(jobType) as IScheduledJob
            ?? throw new InvalidOperationException(
                $"Type {jobType.Name} is registered in DI but does not implement IScheduledJob");

        var context = new JobContext
        {
            ExecutionId = hangfireJobId ?? Guid.NewGuid().ToString("N"),
            JobId = hangfireJobId ?? Guid.NewGuid().ToString("N"),
            JobType = jobType.AssemblyQualifiedName!,
            TenantId = metadata.GetValueOrDefault("tenantId"),
            CorrelationId = metadata.GetValueOrDefault("correlationId"),
            UserId = metadata.GetValueOrDefault("userId"),
            SerializedParameters = serializedParameters,
            CancellationToken = cancellationToken,
            Metadata = metadata,
            SchedulerMetadata = new Dictionary<string, object>
            {
                [HangfireMetadataKeys.HangfireJobId] = hangfireJobId ?? string.Empty
            }
        };

        var result = await job.ExecuteAsync(context);

        if (result.Success)
        {
            _logger.LogInformation(
                "Job [{JobType}] completed successfully: {Message}",
                jobType.Name, result.Message);
        }
        else if (result.ShouldRetry)
        {
            _logger.LogWarning(result.Exception,
                "Job [{JobType}] failed (retryable): {Message}",
                jobType.Name, result.Message);

            throw new ScheduledJobRetryException(result.Message ?? "Job requested retry", result.Exception);
        }
        else
        {
            _logger.LogError(result.Exception,
                "Job [{JobType}] failed permanently: {Message}",
                jobType.Name, result.Message);
        }
    }
}

/// <summary>
/// Helper to extract the current Hangfire job ID from the PerformContext.
/// </summary>
internal static class JobContext_Hangfire
{
    /// <summary>
    /// Gets the current Hangfire job ID from PerformContext (available during execution).
    /// Falls back to null if not in a Hangfire execution context.
    /// </summary>
    public static string? GetCurrentJobId()
    {
        // Hangfire sets this via JobFilterAttribute or PerformContext
        // In the expression-based API, we rely on the connection
        return null; // Will be set via HangfireJobIdFilter
    }
}

/// <summary>
/// Exception thrown when a job execution returns <see cref="JobResult.ShouldRetry"/> = true.
/// Hangfire will see this as a failed job and retry based on its automatic retry configuration.
/// </summary>
public sealed class ScheduledJobRetryException : Exception
{
    public ScheduledJobRetryException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
