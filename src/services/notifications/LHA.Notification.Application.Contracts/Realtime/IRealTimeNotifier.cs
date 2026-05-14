namespace LHA.Notification.Application.Contracts;

public interface IRealTimeNotifier
{
    Task NotifyUserAsync(Guid tenantId, Guid userId, NotificationDto notification);
    Task NotifyGroupAsync(Guid tenantId, string groupName, NotificationDto notification);
    Task NotifyTopicAsync(Guid tenantId, string topicName, NotificationDto notification);
    Task NotifyAllAsync(Guid tenantId, NotificationDto notification);
    Task NotifyUnreadCountAsync(Guid tenantId, Guid userId, int count);
    Task NotifyBatchProgressAsync(Guid tenantId, Guid batchId, BatchProgressDto progress);
    Task AddUserToGroupAsync(Guid tenantId, Guid userId, string groupName);
    Task RemoveUserFromGroupAsync(Guid tenantId, Guid userId, string groupName);
    Task SubscribeToUserAsync(Guid tenantId, Guid userId, INotificationHubClient client);
    Task UnsubscribeFromUserAsync(Guid tenantId, Guid userId, INotificationHubClient client);
}