namespace LHA.Notification.Domain.Shared;

public enum CBatchStatus
{
    /// <summary>
    /// 0 - Draft
    /// </summary>
    Draft,
    /// <summary>
    /// 1 - Scheduled
    /// </summary>
    Scheduled,
    /// <summary>
    /// 2 - Processing
    /// </summary>
    Processing,
    /// <summary>
    /// 3 - Completed
    /// </summary>
    Completed,
    /// <summary>
    /// 4 - PartiallyCompleted
    /// </summary>
    PartiallyCompleted,
    /// <summary>
    /// 5 - Failed
    /// </summary>
    Failed,
    /// <summary>
    /// 6 - Cancelled
    /// </summary>
    Cancelled
}
