namespace LHA.Notification.Domain.Shared;

public enum CDeliveryStatus
{
    Pending = 0,
    Queued = 1,
    Sending = 2,
    Sent = 3,
    Delivered = 4,
    Read = 5,
    Failed = 6,
    Expired = 7,
    Cancelled = 8
}