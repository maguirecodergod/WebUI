namespace LHA.Shared.Contracts.BackgroundJobs;

public class QueueWithTopEnqueuedJobsDto
{
    public string Name { get; set; } = default!;
    public long Length { get; set; }
    public long? Fetched { get; set; }
    public List<EnqueuedJobDto> FirstJobs { get; set; } = new();
}

public class ServerDto
{
    public string Name { get; set; } = default!;
    public int WorkersCount { get; set; }
    public DateTime StartedAt { get; set; }
    public List<string> Queues { get; set; } = new();
    public DateTime? Heartbeat { get; set; }
}

public class JobDetailsDto
{
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
    public List<StateHistoryDto> History { get; set; } = new();
    public DateTime? ExpireAt { get; set; }
}

public class StateHistoryDto
{
    public string StateName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? Reason { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}

public class StatisticsDto
{
    public long Servers { get; set; }
    public long Recurring { get; set; }
    public long Enqueued { get; set; }
    public long Queues { get; set; }
    public long Scheduled { get; set; }
    public long Processing { get; set; }
    public long Succeeded { get; set; }
    public long Failed { get; set; }
    public long Deleted { get; set; }
    public long? Retries { get; set; }
    public long? Awaiting { get; set; }
}

public class FetchedJobDto
{
    public string JobId { get; set; } = default!;
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public string? State { get; set; }
    public DateTime? FetchedAt { get; set; }
}

public class ProcessingJobDto
{
    public string JobId { get; set; } = default!;
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public bool InProcessingState { get; set; }
    public string? ServerId { get; set; }
    public DateTime? StartedAt { get; set; }
}

public class ScheduledJobDto
{
    public string JobId { get; set; } = default!;
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public DateTime EnqueueAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool InScheduledState { get; set; }
}

public class SucceededJobDto
{
    public string JobId { get; set; } = default!;
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public string? Result { get; set; }
    public long? TotalDuration { get; set; }
    public DateTime? SucceededAt { get; set; }
    public bool InSucceededState { get; set; }
}

public class DeletedJobDto
{
    public string JobId { get; set; } = default!;
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool InDeletedState { get; set; }
}
