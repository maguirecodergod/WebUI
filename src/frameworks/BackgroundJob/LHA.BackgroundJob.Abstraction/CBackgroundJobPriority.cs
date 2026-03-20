namespace LHA.BackgroundJob;

/// <summary>
/// Priority of a background job.
/// Higher values indicate higher priority.
/// </summary>
public enum CBackgroundJobPriority : byte
{
    /// <summary>Low priority.</summary>
    Low = 5,

    /// <summary>Below normal priority.</summary>
    BelowNormal = 10,

    /// <summary>Normal (default) priority.</summary>
    Normal = 15,

    /// <summary>Above normal priority.</summary>
    AboveNormal = 20,

    /// <summary>High priority.</summary>
    High = 25
}
