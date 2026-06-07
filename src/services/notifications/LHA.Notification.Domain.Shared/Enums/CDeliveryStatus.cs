namespace LHA.Notification.Domain.Shared;

public enum CDeliveryStatus
{
    /// <summary>
    /// 0 - Pending
    /// </summary>
    Pending = 0,
    /// <summary>
    /// 1 - Queued
    /// </summary>
    Queued = 1,
    /// <summary>
    /// 2 - Sending
    /// </summary>
    Sending = 2,
    /// <summary>
    /// 3 - Sent
    /// </summary>
    Sent = 3,
    /// <summary>
    /// 4 - Delivered
    /// </summary>
    Delivered = 4,
    /// <summary>
    /// 5 - Read
    /// </summary>
    Read = 5,
    /// <summary>
    /// 6 - Failed
    /// </summary>
    Failed = 6,
    /// <summary>
    /// 7 - Expired
    /// </summary>
    Expired = 7,
    /// <summary>
    /// 8 - Cancelled
    /// </summary>
    Cancelled = 8
}
