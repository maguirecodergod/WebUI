using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Hangfire implementation of <see cref="IRecurringJobManager"/>.
/// Maps the scheduler-agnostic recurring job API to Hangfire's RecurringJob manager.
/// </summary>
public sealed class HangfireRecurringJobManager : IRecurringJobManager
{
    private readonly global::Hangfire.IRecurringJobManager _hangfireManager;
    private readonly HangfireSchedulingOptions _options;
    private readonly ILogger<HangfireRecurringJobManager> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public HangfireRecurringJobManager(
        global::Hangfire.IRecurringJobManager hangfireManager,
        Microsoft.Extensions.Options.IOptions<HangfireSchedulingOptions> options,
        ILogger<HangfireRecurringJobManager> logger)
    {
        _hangfireManager = hangfireManager;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task AddOrUpdateAsync<TJob>(
        string recurringJobId,
        string cronExpression,
        RecurringJobOptions? options = null,
        CancellationToken cancellationToken = default) where TJob : IScheduledJob
    {
        var queue = options?.Queue ?? _options.DefaultQueue;
        var timeZone = ResolveTimeZone(options?.TimeZoneId ?? "UTC");
        var serializedParams = options?.Parameters is not null
            ? JsonSerializer.Serialize(options.Parameters, options.Parameters.GetType(), JsonOptions)
            : null;

        var metadata = BuildMetadata(options);

        _hangfireManager.AddOrUpdate<HangfireJobExecutor>(
            recurringJobId,
            queue,
            executor => executor.ExecuteAsync(
                typeof(TJob).AssemblyQualifiedName!,
                serializedParams,
                metadata,
                CancellationToken.None),
            cronExpression,
            new global::Hangfire.RecurringJobOptions
            {
                TimeZone = timeZone,
                MisfireHandling = MapMisfirePolicy(options?.MisfirePolicy ?? MisfirePolicy.FireOnceNow)
            });

        _logger.LogInformation(
            "Recurring job [{RecurringJobId}] registered: [{JobType}] cron [{Cron}] queue [{Queue}] tz [{TimeZone}]",
            recurringJobId, typeof(TJob).Name, cronExpression, queue, timeZone.Id);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string recurringJobId, CancellationToken cancellationToken = default)
    {
        _hangfireManager.RemoveIfExists(recurringJobId);

        _logger.LogInformation("Recurring job [{RecurringJobId}] removed", recurringJobId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task TriggerAsync(string recurringJobId, CancellationToken cancellationToken = default)
    {
        _hangfireManager.Trigger(recurringJobId);

        _logger.LogInformation("Recurring job [{RecurringJobId}] triggered manually", recurringJobId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string recurringJobId, CancellationToken cancellationToken = default)
    {
        using var connection = JobStorage.Current.GetConnection();
        var recurringJobIds = connection.GetAllItemsFromSet("recurring-jobs");
        var exists = recurringJobIds.Any(id =>
            string.Equals(id, recurringJobId, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RecurringJobDefinition>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var connection = JobStorage.Current.GetConnection();
        var recurringJobIds = connection.GetAllItemsFromSet("recurring-jobs");

        var definitions = new List<RecurringJobDefinition>();
        foreach (var id in recurringJobIds)
        {
            var data = connection.GetAllEntriesFromHash($"recurring-job:{id}");
            if (data is null) continue;

            data.TryGetValue("Cron", out var cron);
            data.TryGetValue("Job", out var jobTypeRaw);
            data.TryGetValue("Queue", out var queue);
            data.TryGetValue("TimeZoneId", out var tz);
            data.TryGetValue("NextExecution", out var nextExec);
            data.TryGetValue("LastExecution", out var lastExec);

            definitions.Add(new RecurringJobDefinition
            {
                RecurringJobId = id,
                CronExpression = cron ?? string.Empty,
                JobType = jobTypeRaw ?? "unknown",
                Queue = queue,
                TimeZoneId = tz ?? "UTC",
                LastExecutedAt = DateTimeOffset.TryParse(lastExec, out var last) ? last : null,
                NextExecutionAt = DateTimeOffset.TryParse(nextExec, out var next) ? next : null,
                Status = RecurringJobStatus.Active
            });
        }

        return Task.FromResult<IReadOnlyList<RecurringJobDefinition>>(definitions);
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    private static MisfireHandlingMode MapMisfirePolicy(MisfirePolicy policy) => policy switch
    {
        MisfirePolicy.FireOnceNow => MisfireHandlingMode.Relaxed,
        MisfirePolicy.IgnoreMisfire => MisfireHandlingMode.Ignorable,
        MisfirePolicy.FireAll => MisfireHandlingMode.Strict,
        _ => MisfireHandlingMode.Relaxed
    };

    private Dictionary<string, string> BuildMetadata(RecurringJobOptions? options)
    {
        var metadata = new Dictionary<string, string>();
        if (options is null) return metadata;

        if (!string.IsNullOrEmpty(options.TenantId))
            metadata["tenantId"] = options.TenantId;

        foreach (var kvp in options.Metadata)
            metadata[kvp.Key] = kvp.Value;

        return metadata;
    }
}
