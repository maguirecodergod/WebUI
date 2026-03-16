using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Hangfire implementation of <see cref="IJobScheduler"/>.
/// Maps the scheduler-agnostic API to Hangfire's BackgroundJob static methods.
/// </summary>
public sealed class HangfireJobScheduler : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly HangfireSchedulingOptions _options;
    private readonly ILogger<HangfireJobScheduler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public HangfireJobScheduler(
        IBackgroundJobClient backgroundJobClient,
        IOptions<HangfireSchedulingOptions> options,
        ILogger<HangfireJobScheduler> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string> EnqueueAsync<TJob>(
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob
    {
        var queue = options?.Queue ?? _options.DefaultQueue;
        var serializedParams = SerializeParameters(parameters);
        var metadata = BuildMetadata(options);

        var jobId = _backgroundJobClient.Enqueue<HangfireJobExecutor>(
            queue,
            executor => executor.ExecuteAsync(
                typeof(TJob).AssemblyQualifiedName!,
                serializedParams,
                metadata,
                CancellationToken.None));

        _logger.LogInformation(
            "Enqueued job [{JobType}] → Hangfire ID [{JobId}] queue [{Queue}]",
            typeof(TJob).Name, jobId, queue);

        return Task.FromResult(jobId);
    }

    /// <inheritdoc />
    public Task<string> ScheduleAsync<TJob>(
        TimeSpan delay,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob
    {
        var queue = options?.Queue ?? _options.DefaultQueue;
        var serializedParams = SerializeParameters(parameters);
        var metadata = BuildMetadata(options);

        var jobId = _backgroundJobClient.Schedule<HangfireJobExecutor>(
            queue,
            executor => executor.ExecuteAsync(
                typeof(TJob).AssemblyQualifiedName!,
                serializedParams,
                metadata,
                CancellationToken.None),
            delay);

        _logger.LogInformation(
            "Scheduled job [{JobType}] → Hangfire ID [{JobId}] delay [{Delay}]",
            typeof(TJob).Name, jobId, delay);

        return Task.FromResult(jobId);
    }

    /// <inheritdoc />
    public Task<string> ScheduleAsync<TJob>(
        DateTimeOffset enqueueAt,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob
    {
        var queue = options?.Queue ?? _options.DefaultQueue;
        var serializedParams = SerializeParameters(parameters);
        var metadata = BuildMetadata(options);

        var jobId = _backgroundJobClient.Schedule<HangfireJobExecutor>(
            queue,
            executor => executor.ExecuteAsync(
                typeof(TJob).AssemblyQualifiedName!,
                serializedParams,
                metadata,
                CancellationToken.None),
            enqueueAt);

        _logger.LogInformation(
            "Scheduled job [{JobType}] → Hangfire ID [{JobId}] at [{EnqueueAt}]",
            typeof(TJob).Name, jobId, enqueueAt);

        return Task.FromResult(jobId);
    }

    /// <inheritdoc />
    public Task<string> ContinueWithAsync<TJob>(
        string parentJobId,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob
    {
        var queue = options?.Queue ?? _options.DefaultQueue;
        var serializedParams = SerializeParameters(parameters);
        var metadata = BuildMetadata(options);

        var jobId = _backgroundJobClient.ContinueJobWith<HangfireJobExecutor>(
            parentJobId,
            queue,
            executor => executor.ExecuteAsync(
                typeof(TJob).AssemblyQualifiedName!,
                serializedParams,
                metadata,
                CancellationToken.None));

        _logger.LogInformation(
            "Continuation job [{JobType}] → Hangfire ID [{JobId}] after parent [{ParentJobId}]",
            typeof(TJob).Name, jobId, parentJobId);

        return Task.FromResult(jobId);
    }

    /// <inheritdoc />
    public Task<bool> CancelAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var result = _backgroundJobClient.Delete(jobId);

        _logger.LogInformation(
            "Cancel job [{JobId}] → {Result}",
            jobId, result ? "deleted" : "not found");

        return Task.FromResult(result);
    }

    private static string? SerializeParameters(object? parameters)
    {
        if (parameters is null) return null;
        return JsonSerializer.Serialize(parameters, parameters.GetType(), JsonOptions);
    }

    private Dictionary<string, string> BuildMetadata(JobOptions? options)
    {
        var metadata = new Dictionary<string, string>();

        if (options is null) return metadata;

        if (!string.IsNullOrEmpty(options.TenantId))
            metadata["tenantId"] = options.TenantId;
        if (!string.IsNullOrEmpty(options.CorrelationId))
            metadata["correlationId"] = options.CorrelationId;
        if (!string.IsNullOrEmpty(options.UserId))
            metadata["userId"] = options.UserId;

        foreach (var kvp in options.Metadata)
            metadata[kvp.Key] = kvp.Value;

        return metadata;
    }
}
