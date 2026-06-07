namespace LHA.Shared.Contracts.BackgroundJobs;

/// <summary>
/// Represents a background job queue with its top enqueued jobs for monitoring.
/// </summary>
public class QueueWithTopEnqueuedJobsDto
{
    /// <summary>
    /// Name of the queue.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Total number of jobs currently waiting in the queue.
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// Number of jobs that have been fetched by a server but not yet completed.
    /// </summary>
    public long? Fetched { get; set; }

    /// <summary>
    /// First enqueued jobs in the queue for quick preview.
    /// </summary>
    public List<EnqueuedJobDto> FirstJobs { get; set; } = new();
}

/// <summary>
/// Represents a background job processing server instance.
/// </summary>
public class ServerDto
{
    /// <summary>
    /// Unique server instance name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Number of active worker threads on this server.
    /// </summary>
    public int WorkersCount { get; set; }

    /// <summary>
    /// Timestamp when the server instance was started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// List of queue names this server is configured to process.
    /// </summary>
    public List<string> Queues { get; set; } = new();

    /// <summary>
    /// Last heartbeat timestamp indicating server liveness.
    /// </summary>
    public DateTime? Heartbeat { get; set; }
}

/// <summary>
/// Detailed information about a specific background job.
/// </summary>
public class JobDetailsDto
{
    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Timestamp when the job was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Arbitrary key-value properties attached to the job.
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    /// <summary>
    /// Chronological list of state transitions the job has gone through.
    /// </summary>
    public List<StateHistoryDto> History { get; set; } = new();

    /// <summary>
    /// Timestamp when the job is scheduled to expire.
    /// </summary>
    public DateTime? ExpireAt { get; set; }
}

/// <summary>
/// Represents a single state transition entry in a job's history.
/// </summary>
public class StateHistoryDto
{
    /// <summary>
    /// Name of the state (e.g., <c>Enqueued</c>, <c>Processing</c>, <c>Succeeded</c>).
    /// </summary>
    public string StateName { get; set; } = default!;

    /// <summary>
    /// Timestamp when this state was applied.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Optional reason describing why the state transition occurred.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Additional key-value data associated with this state transition.
    /// </summary>
    public Dictionary<string, string> Data { get; set; } = new();
}

/// <summary>
/// Aggregate statistics for the background job system dashboard.
/// </summary>
public class StatisticsDto
{
    /// <summary>
    /// Total number of active processing servers.
    /// </summary>
    public long Servers { get; set; }

    /// <summary>
    /// Total number of registered recurring jobs.
    /// </summary>
    public long Recurring { get; set; }

    /// <summary>
    /// Total number of jobs currently enqueued across all queues.
    /// </summary>
    public long Enqueued { get; set; }

    /// <summary>
    /// Total number of registered queues.
    /// </summary>
    public long Queues { get; set; }

    /// <summary>
    /// Total number of jobs scheduled for future execution.
    /// </summary>
    public long Scheduled { get; set; }

    /// <summary>
    /// Total number of jobs currently being processed.
    /// </summary>
    public long Processing { get; set; }

    /// <summary>
    /// Total number of jobs that have completed successfully.
    /// </summary>
    public long Succeeded { get; set; }

    /// <summary>
    /// Total number of jobs that have failed.
    /// </summary>
    public long Failed { get; set; }

    /// <summary>
    /// Total number of jobs that have been deleted.
    /// </summary>
    public long Deleted { get; set; }

    /// <summary>
    /// Number of jobs currently awaiting retry.
    /// </summary>
    public long? Retries { get; set; }

    /// <summary>
    /// Number of jobs in the awaiting state, typically waiting for a parent job to complete.
    /// </summary>
    public long? Awaiting { get; set; }
}

/// <summary>
/// Represents a job that has been fetched from the queue by a worker.
/// </summary>
public class FetchedJobDto
{
    /// <summary>
    /// Unique identifier of the job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Current state name of the job.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Timestamp when the job was fetched from the queue.
    /// </summary>
    public DateTime? FetchedAt { get; set; }
}

/// <summary>
/// Represents a job that is currently being processed by a server.
/// </summary>
public class ProcessingJobDto
{
    /// <summary>
    /// Unique identifier of the job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Indicates whether the job is still in the <c>Processing</c> state.
    /// </summary>
    public bool InProcessingState { get; set; }

    /// <summary>
    /// Identifier of the server currently processing this job.
    /// </summary>
    public string? ServerId { get; set; }

    /// <summary>
    /// Timestamp when the job started processing.
    /// </summary>
    public DateTime? StartedAt { get; set; }
}

/// <summary>
/// Represents a job that is scheduled for future execution.
/// </summary>
public class ScheduledJobDto
{
    /// <summary>
    /// Unique identifier of the job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Timestamp when the job is scheduled to be enqueued.
    /// </summary>
    public DateTime EnqueueAt { get; set; }

    /// <summary>
    /// Timestamp when the job was placed in the scheduled state.
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Indicates whether the job is still in the <c>Scheduled</c> state.
    /// </summary>
    public bool InScheduledState { get; set; }
}

/// <summary>
/// Represents a job that has completed successfully.
/// </summary>
public class SucceededJobDto
{
    /// <summary>
    /// Unique identifier of the job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Result output produced by the job, if any.
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Total execution duration in milliseconds.
    /// </summary>
    public long? TotalDuration { get; set; }

    /// <summary>
    /// Timestamp when the job transitioned to the <c>Succeeded</c> state.
    /// </summary>
    public DateTime? SucceededAt { get; set; }

    /// <summary>
    /// Indicates whether the job is still in the <c>Succeeded</c> state.
    /// </summary>
    public bool InSucceededState { get; set; }
}

/// <summary>
/// Represents a job that has been deleted.
/// </summary>
public class DeletedJobDto
{
    /// <summary>
    /// Unique identifier of the job.
    /// </summary>
    public string JobId { get; set; } = default!;

    /// <summary>
    /// Fully qualified method signature executed by the job.
    /// </summary>
    public string? JobMethod { get; set; }

    /// <summary>
    /// Exception message encountered when loading the job, if any.
    /// </summary>
    public string? LoadExceptionMessage { get; set; }

    /// <summary>
    /// Timestamp when the job was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Indicates whether the job is still in the <c>Deleted</c> state.
    /// </summary>
    public bool InDeletedState { get; set; }
}
