namespace LHA.Shared.Contracts.BackgroundJobs;

/// <summary>
/// Represents a job waiting in a background job queue.
/// </summary>
public class EnqueuedJobDto
{
    /// <summary>
    /// Unique identifier of the enqueued job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature to be executed by the job.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Current state name of the job (e.g., <c>Enqueued</c>).
    /// </summary>
    public string? State { get; set; }
}
