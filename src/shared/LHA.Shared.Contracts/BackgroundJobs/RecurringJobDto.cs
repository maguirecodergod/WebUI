namespace LHA.Shared.Contracts.BackgroundJobs;

public class RecurringJobDto
{
    public string Id { get; set; } = default!;
    public string Cron { get; set; } = default!;
    public string Queue { get; set; } = default!;

    private DateTime? _nextExecution;
    public DateTime? NextExecution
    {
        get => _nextExecution?.ToLocalTime();
        set => _nextExecution = value;
    }

    private DateTime? _lastExecution;
    public DateTime? LastExecution
    {
        get => _lastExecution?.ToLocalTime();
        set => _lastExecution = value;
    }

    public string? LastJobState { get; set; }
    public string? LastJobId { get; set; }
    public string? TimeZoneId { get; set; }
    
    private DateTime? _createdAt;
    public DateTime? CreatedAt
    {
        get => _createdAt?.ToLocalTime();
        set => _createdAt = value;
    }

    public string? Error { get; set; }
    public int RetryAttempt { get; set; }
    public bool Removed { get; set; }
    public string? JobMethod { get; set; }
    public string? LoadExceptionMessage { get; set; }
}
