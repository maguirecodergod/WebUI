namespace LHA.Notification.Domain.Shared;

public enum CRateLimitWindow
{
    /// <summary>
    /// 0 - Second
    /// </summary>
    Second,
    /// <summary>
    /// 1 - Minute
    /// </summary>
    Minute,
    /// <summary>
    /// 2 - Hour
    /// </summary>
    Hour,
    /// <summary>
    /// 3 - Day
    /// </summary>
    Day
}
