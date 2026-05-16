using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using LHA.Notification.Application.Contracts;

namespace LHA.Notification.Infrastructure;

internal sealed class NotificationHubContext : INotificationHubContext
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationHubContext> _logger;

    public NotificationHubContext(IHubContext<NotificationHub> hubContext, ILogger<NotificationHubContext> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(Guid tenantId, Guid userId, NotificationDto notification)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync("notification.received", notification);
        _logger.LogInformation("Sent notification to user {UserId}", userId);
    }

    public async Task SendNotificationToGroupAsync(Guid tenantId, string groupName, NotificationDto notification)
    {
        var group = $"{tenantId}:group:{groupName}";
        await _hubContext.Clients.Group(group).SendAsync("notification.received", notification);
        _logger.LogInformation("Sent notification to group {GroupName}", groupName);
    }

    public async Task SendNotificationReadAsync(Guid tenantId, Guid userId, Guid notificationId)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync("notification.read", notificationId);
    }

    public async Task SendNotificationDeletedAsync(Guid tenantId, Guid userId, Guid notificationId)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync("notification.deleted", notificationId);
    }

    public async Task SendUnreadCountUpdatedAsync(Guid tenantId, Guid userId, int unreadCount)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync("unread.count.updated", unreadCount);
    }

    public async Task SendBatchProgressUpdatedAsync(Guid tenantId, Guid batchId, BatchProgressDto progress)
    {
        await _hubContext.Clients.Group($"tenant:{tenantId}").SendAsync("batch.progress.updated", progress);
    }

    public async Task SendToTopicAsync(Guid tenantId, string topicName, NotificationDto notification)
    {
        var group = $"{tenantId}:topic:{topicName}";
        await _hubContext.Clients.Group(group).SendAsync("notification.received", notification);
    }

    public async Task BroadcastAsync(Guid tenantId, NotificationDto notification)
    {
        await _hubContext.Clients.Group($"tenant:{tenantId}").SendAsync("notification.received", notification);
    }
}
