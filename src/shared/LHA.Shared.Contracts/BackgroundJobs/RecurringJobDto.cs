namespace LHA.Shared.Contracts.BackgroundJobs;

/// <summary>
/// Represents a recurring background job with its schedule and execution metadata.
/// </summary>
public class RecurringJobDto
{
    /// <summary>
    /// Unique identifier of the recurring job.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Cron expression defining the recurring schedule.
    /// </summary>
    public string Cron { get; set; } = default!;

    /// <summary>
    /// Name of the queue the recurring job will be enqueued to.
    /// </summary>
    public string Queue { get; set; } = default!;

    /// <summary>
    /// Estimated timestamp of the next scheduled execution, converted to local time.
    /// </summary>
    private DateTime? _nextExecution;
    public DateTime? NextExecution
    {
        get => _nextExecution?.ToLocalTime();
        set => _nextExecution = value;
    }

    /// <summary>
    /// Timestamp of the most recent execution, converted to local time.
    /// </summary>
    private DateTime? _lastExecution;
    public DateTime? LastExecution
    {
        get => _lastExecution?.ToLocalTime();
        set => _lastExecution = value;
    }

    /// <summary>
    /// State name of the last executed job instance (e.g., <c>Succeeded</c>, <c>Failed</c>).
    /// </summary>
    public string? LastJobState { get; set; }

    /// <summary>
    /// Job identifier of the last executed instance.
    /// </summary>
    public string? LastJobId { get; set; }

    /// <summary>
    /// Time zone identifier used for scheduling (e.g., <c>UTC</c>, <c>SE Asia Standard Time</c>).
    /// </summary>
    public string? TimeZoneId { get; set; }

    /// <summary>
    /// Timestamp when the recurring job was created, converted to local time.
    /// </summary>
    private DateTime? _createdAt;
    public DateTime? CreatedAt
    {
        get => _createdAt?.ToLocalTime();
        set => _createdAt = value;
    }

    /// <summary>
    /// Error message from the last execution, if the job failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Number of retry attempts made for the last execution.
    /// </summary>
    public int RetryAttempt { get; set; }

    /// <summary>
    /// Indicates whether the recurring job has been removed from the schedule.
    /// </summary>
    public bool Removed { get; set; }

    /// <summary>
    /// Fully qualified method signature executed by the recurring job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }
}
