namespace LHA.Notification.Domain.Shared;

public enum CBatchStatus
{
    Draft,
    Scheduled,
    Processing,
    Completed,
    PartiallyCompleted,
    Failed,
    Cancelled
}
