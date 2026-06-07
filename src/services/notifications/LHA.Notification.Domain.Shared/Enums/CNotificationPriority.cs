namespace LHA.Notification.Domain.Shared;

/// <summary>
/// Notification priority.
/// </summary>
public enum CNotificationPriority
{
    /// <summary>
    /// 0 - Low: 0 - Low priority.
    /// </summary>
    Low = 0,
    /// <summary>
    /// 1 - Normal: 1 - Normal priority.
    /// </summary>
    Normal = 1,
    /// <summary>
    /// 2 - High: 2 - High priority.
    /// </summary>
    High = 2,
    /// <summary>
    /// 3 - Critical: 3 - Critical priority.
    /// </summary>
    Critical = 3
}
