using LHA.Notification.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure;

internal sealed class RealTimeNotifier(
    INotificationHubContext hubContext,
    ILogger<RealTimeNotifier> logger) : IRealTimeNotifier
{
    public async Task NotifyUserAsync(Guid tenantId, Guid userId, NotificationDto notification)
    {
        logger.LogInformation("RealTimeNotifier: Notifying user {UserId} in tenant {TenantId}", userId, tenantId);
        await hubContext.SendNotificationToUserAsync(tenantId, userId, notification);
    }

    public async Task NotifyGroupAsync(Guid tenantId, string groupName, NotificationDto notification)
    {
        await hubContext.SendNotificationToGroupAsync(tenantId, groupName, notification);
    }

    public async Task NotifyTopicAsync(Guid tenantId, string topicName, NotificationDto notification)
    {
        await hubContext.SendToTopicAsync(tenantId, topicName, notification);
    }

    public async Task NotifyAllAsync(Guid tenantId, NotificationDto notification)
    {
        await hubContext.BroadcastAsync(tenantId, notification);
    }

    public async Task NotifyUnreadCountAsync(Guid tenantId, Guid userId, int count)
    {
        await hubContext.SendUnreadCountUpdatedAsync(tenantId, userId, count);
    }

    public async Task NotifyBatchProgressAsync(Guid tenantId, Guid batchId, BatchProgressDto progress)
    {
        await hubContext.SendBatchProgressUpdatedAsync(tenantId, batchId, progress);
    }

    public Task AddUserToGroupAsync(Guid tenantId, Guid userId, string groupName)
    {
        // HubContext can't easily add a specific user to a group without their ConnectionId 
        // unless they are already connected. Usually done via Hub.
        return Task.CompletedTask;
    }

    public Task RemoveUserFromGroupAsync(Guid tenantId, Guid userId, string groupName)
    {
        return Task.CompletedTask;
    }

    public Task SubscribeToUserAsync(Guid tenantId, Guid userId, INotificationHubClient client)
    {
        return Task.CompletedTask;
    }

    public Task UnsubscribeFromUserAsync(Guid tenantId, Guid userId, INotificationHubClient client)
    {
        return Task.CompletedTask;
    }
}
