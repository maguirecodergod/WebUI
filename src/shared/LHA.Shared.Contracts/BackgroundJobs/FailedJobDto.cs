namespace LHA.Shared.Contracts.BackgroundJobs;

public class FailedJobDto
{
    public string JobId { get; set; } = default!;
    public string? Method { get; set; }
    public string? ExceptionDetails { get; set; }
    public DateTime? FailedAt { get; set; }
}
