namespace LHA.BackgroundJob;

/// <summary>
/// Exception thrown when a background job execution fails.
/// Wraps the original exception with job metadata for diagnostics.
/// </summary>
public sealed class BackgroundJobExecutionException : Exception
{
    /// <summary>Assembly-qualified type name of the failed job.</summary>
    public string? JobType { get; init; }

    /// <summary>The job arguments that were passed to the job.</summary>
    public object? JobArgs { get; init; }

    public BackgroundJobExecutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
