namespace LHA.Scheduling;

/// <summary>
/// Result of a job execution.
/// </summary>
public sealed record JobResult
{
    /// <summary>Whether the job completed successfully.</summary>
    public required bool Success { get; init; }

    /// <summary>Human-readable message describing the outcome.</summary>
    public string? Message { get; init; }

    /// <summary>Exception that caused the failure (if any).</summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// If true and <see cref="Success"/> is false, the scheduler should retry
    /// according to the configured retry policy. If false, the failure is permanent.
    /// </summary>
    public bool ShouldRetry { get; init; }

    /// <summary>Optional data attached to the result for logging or auditing.</summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();

    /// <summary>Creates a successful result.</summary>
    public static JobResult Ok(string? message = null)
        => new() { Success = true, Message = message ?? "Job completed successfully" };

    /// <summary>Creates a permanent failure result (no retry).</summary>
    public static JobResult Fail(string message, Exception? exception = null)
        => new() { Success = false, Message = message, Exception = exception, ShouldRetry = false };

    /// <summary>Creates a transient failure result (should be retried).</summary>
    public static JobResult Retry(string message, Exception? exception = null)
        => new() { Success = false, Message = message, Exception = exception, ShouldRetry = true };
}
