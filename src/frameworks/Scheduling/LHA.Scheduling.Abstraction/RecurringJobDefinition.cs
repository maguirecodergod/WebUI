namespace LHA.Scheduling;

/// <summary>
/// Options for a recurring job registration.
/// </summary>
public sealed class RecurringJobOptions
{
    /// <summary>Optional static parameters passed to every execution of the recurring job.</summary>
    public object? Parameters { get; set; }

    /// <summary>Tenant ID for multi-tenant job isolation.</summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Queue name for the recurring job executions.
    /// If null, the scheduler's default queue is used.
    /// </summary>
    public string? Queue { get; set; }

    /// <summary>
    /// Time zone ID for cron expression evaluation (e.g. "Asia/Ho_Chi_Minh", "UTC").
    /// Defaults to UTC if not specified.
    /// </summary>
    public string TimeZoneId { get; set; } = "UTC";

    /// <summary>
    /// Display name for the recurring job in monitoring dashboards.
    /// If null, derived from the recurring job ID.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>Max retry attempts per execution. If null, the scheduler default is used.</summary>
    public int? MaxRetries { get; set; }

    /// <summary>
    /// Misfire handling strategy when a scheduled fire time is missed
    /// (e.g. server was down during the scheduled time).
    /// </summary>
    public MisfirePolicy MisfirePolicy { get; set; } = MisfirePolicy.FireOnceNow;

    /// <summary>Additional metadata passed through to the job context on each execution.</summary>
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Read-only snapshot of a registered recurring job for listing/display purposes.
/// </summary>
public sealed record RecurringJobDefinition
{
    /// <summary>Unique recurring job ID.</summary>
    public required string RecurringJobId { get; init; }

    /// <summary>Cron expression defining the schedule.</summary>
    public required string CronExpression { get; init; }

    /// <summary>Fully qualified .NET type name of the job.</summary>
    public required string JobType { get; init; }

    /// <summary>Queue the job executions are dispatched to.</summary>
    public string? Queue { get; init; }

    /// <summary>Time zone for cron evaluation.</summary>
    public string TimeZoneId { get; init; } = "UTC";

    /// <summary>When the job last executed (null if never).</summary>
    public DateTimeOffset? LastExecutedAt { get; init; }

    /// <summary>When the job is next scheduled to execute (null if paused/removed).</summary>
    public DateTimeOffset? NextExecutionAt { get; init; }

    /// <summary>Current status.</summary>
    public RecurringJobStatus Status { get; init; } = RecurringJobStatus.Active;
}

/// <summary>
/// Status of a recurring job registration.
/// </summary>
public enum RecurringJobStatus
{
    /// <summary>Actively scheduled — will fire at the next cron interval.</summary>
    Active,

    /// <summary>Paused — will not fire until resumed.</summary>
    Paused,

    /// <summary>Removed/completed — no longer scheduled.</summary>
    Removed
}

/// <summary>
/// Defines how the scheduler should handle missed fire times.
/// </summary>
public enum MisfirePolicy
{
    /// <summary>
    /// Fire once immediately, then resume normal cron schedule.
    /// Best for: idempotent jobs where catching up matters.
    /// </summary>
    FireOnceNow,

    /// <summary>
    /// Ignore the missed fire and wait for the next scheduled time.
    /// Best for: jobs where stale executions have no value.
    /// </summary>
    IgnoreMisfire,

    /// <summary>
    /// Fire all missed executions sequentially, then resume normal schedule.
    /// Best for: jobs where every execution matters (e.g. daily reports).
    /// Warning: can cause burst load after prolonged downtime.
    /// </summary>
    FireAll
}
