namespace LHA.BackgroundJob;

/// <summary>
/// Priority of a background job.
/// Higher values indicate higher priority.
/// </summary>
public enum CBackgroundJobPriority : byte
{
    /// <summary>
    /// 5 - Low: Low priority.
    /// </summary>
    Low = 5,

    /// <summary>
    /// 10 - BelowNormal: Below normal priority.
    /// </summary>
    BelowNormal = 10,

    /// <summary>
    /// 15 - Normal: Normal (default) priority.
    /// </summary>
    Normal = 15,

    /// <summary>
    /// 20 - AboveNormal: Above normal priority.
    /// </summary>
    AboveNormal = 20,

    /// <summary>
    /// 25 - High: High priority.
    /// </summary>
    High = 25
}
