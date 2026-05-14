namespace LHA.Notification.Application.Contracts;

public interface INotificationHubContext
{
    Task SendNotificationToUserAsync(Guid tenantId, Guid userId, NotificationDto notification);
    Task SendNotificationToGroupAsync(Guid tenantId, string groupName, NotificationDto notification);
    Task SendNotificationReadAsync(Guid tenantId, Guid userId, Guid notificationId);
    Task SendNotificationDeletedAsync(Guid tenantId, Guid userId, Guid notificationId);
    Task SendUnreadCountUpdatedAsync(Guid tenantId, Guid userId, int unreadCount);
    Task SendBatchProgressUpdatedAsync(Guid tenantId, Guid batchId, BatchProgressDto progress);
    Task SendToTopicAsync(Guid tenantId, string topicName, NotificationDto notification);
    Task BroadcastAsync(Guid tenantId, NotificationDto notification);
}