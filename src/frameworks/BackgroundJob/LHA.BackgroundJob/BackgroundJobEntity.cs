namespace LHA.BackgroundJob;

/// <summary>
/// Represents a persisted background job in the job store.
/// </summary>
public sealed class BackgroundJobEntity
{
    /// <summary>Unique job identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Application name for multi-app isolation.</summary>
    public string? ApplicationName { get; set; }

    /// <summary>Job name (resolved from the args type via <see cref="BackgroundJobNameAttribute"/>).</summary>
    public required string JobName { get; set; }

    /// <summary>Serialized job arguments (JSON).</summary>
    public required string JobArgs { get; set; }

    /// <summary>Job priority (higher = more important).</summary>
    public CBackgroundJobPriority Priority { get; set; } = CBackgroundJobPriority.Normal;

    /// <summary>Number of execution attempts.</summary>
    public short TryCount { get; set; }

    /// <summary>When the job was created (UTC).</summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>Earliest time the job may next be executed (UTC).</summary>
    public DateTimeOffset NextTryTime { get; set; }

    /// <summary>Last execution attempt time (UTC).</summary>
    public DateTimeOffset? LastTryTime { get; set; }

    /// <summary>If true, the job has permanently failed and will not be retried.</summary>
    public bool IsAbandoned { get; set; }
}
