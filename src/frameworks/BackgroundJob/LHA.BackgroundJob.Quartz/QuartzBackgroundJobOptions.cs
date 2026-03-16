namespace LHA.BackgroundJob.Quartz;

/// <summary>
/// Options for the Quartz-backed background job system.
/// </summary>
public sealed class QuartzBackgroundJobOptions
{
    /// <summary>
    /// The Quartz job group for background jobs.
    /// Default: <c>"LHABackgroundJobs"</c>.
    /// </summary>
    public string JobGroup { get; set; } = "LHABackgroundJobs";

    /// <summary>
    /// The Quartz trigger group for background job triggers.
    /// Default: <c>"LHABackgroundJobTriggers"</c>.
    /// </summary>
    public string TriggerGroup { get; set; } = "LHABackgroundJobTriggers";

    /// <summary>
    /// Whether Quartz should request recovery for failed jobs.
    /// When <see langword="true"/>, the job will be re-executed after a scheduler crash.
    /// Default: <see langword="true"/>.
    /// </summary>
    public bool RequestRecovery { get; set; } = true;
}
