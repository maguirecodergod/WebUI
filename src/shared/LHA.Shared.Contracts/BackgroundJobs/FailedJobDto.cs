namespace LHA.Shared.Contracts.BackgroundJobs;

/// <summary>
/// Represents a background job that has failed during execution.
/// </summary>
public class FailedJobDto
{
    /// <summary>
    /// Unique identifier of the failed job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature that was executed by the job.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Full exception details including stack trace from the failure.
    /// </summary>
    public string? ExceptionDetails { get; set; }

    /// <summary>
    /// Timestamp when the job transitioned to the <c>Failed</c> state.
    /// </summary>
    public DateTime? FailedAt { get; set; }
}
