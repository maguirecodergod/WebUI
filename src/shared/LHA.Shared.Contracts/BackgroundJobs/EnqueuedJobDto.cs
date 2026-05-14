namespace LHA.Shared.Contracts.BackgroundJobs;

public class EnqueuedJobDto
{
    public string JobId { get; set; } = default!;
    public string? Method { get; set; }
    public string? State { get; set; }
}
