namespace LHA.Notification.Application.Contracts;

public interface IRealTimeClient
{
    Task NotificationReceived(NotificationDto notification);
    Task NotificationRead(Guid notificationId);
    Task NotificationDeleted(Guid notificationId);
    Task UnreadCountUpdated(int count);
    Task BatchProgressUpdated(BatchProgressDto progress);
}