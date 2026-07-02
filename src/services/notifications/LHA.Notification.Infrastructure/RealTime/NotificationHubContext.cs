using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using LHA.Notification.Application.Contracts;
using LHA.Shared.Contracts.Realtime;

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
        await _hubContext.Clients.Group(groupName).SendAsync(SignalRHubMethodNames.NotificationEvents.Received, notification);
        _logger.LogInformation("Sent notification to user {UserId}", userId);
    }

    public async Task SendNotificationToGroupAsync(Guid tenantId, string groupName, NotificationDto notification)
    {
        var group = $"{tenantId}:group:{groupName}";
        await _hubContext.Clients.Group(group).SendAsync(SignalRHubMethodNames.NotificationEvents.Received, notification);
        _logger.LogInformation("Sent notification to group {GroupName}", groupName);
    }

    public async Task SendNotificationReadAsync(Guid tenantId, Guid userId, Guid notificationId)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync(SignalRHubMethodNames.NotificationEvents.Read, notificationId);
    }

    public async Task SendNotificationDeletedAsync(Guid tenantId, Guid userId, Guid notificationId)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync(SignalRHubMethodNames.NotificationEvents.Deleted, notificationId);
    }

    public async Task SendUnreadCountUpdatedAsync(Guid tenantId, Guid userId, int unreadCount)
    {
        var groupName = $"{tenantId}:{userId}";
        await _hubContext.Clients.Group(groupName).SendAsync(SignalRHubMethodNames.NotificationEvents.UnreadCountUpdated, unreadCount);
    }

    public async Task SendBatchProgressUpdatedAsync(Guid tenantId, Guid batchId, BatchProgressDto progress)
    {
        await _hubContext.Clients.Group($"tenant:{tenantId}").SendAsync(SignalRHubMethodNames.BatchEvents.ProgressUpdated, progress);
    }

    public async Task SendToTopicAsync(Guid tenantId, string topicName, NotificationDto notification)
    {
        var group = $"{tenantId}:topic:{topicName}";
        await _hubContext.Clients.Group(group).SendAsync(SignalRHubMethodNames.NotificationEvents.Received, notification);
    }

    public async Task BroadcastAsync(Guid tenantId, NotificationDto notification)
    {
        await _hubContext.Clients.Group($"tenant:{tenantId}").SendAsync(SignalRHubMethodNames.NotificationEvents.Received, notification);
    }

    public async Task SendSecurityStateChangedToUserAsync(string userId, object payload, CancellationToken ct = default)
    {
        await _hubContext.Clients.Group(HubGroupNames.User(userId)).SendAsync(SignalRHubMethodNames.SecurityEvents.ForceLogout, payload, ct);
        _logger.LogInformation("Sent security state change to user {UserId}", userId);
    }

    public async Task SendSecurityStateChangedToRoleAsync(string roleName, object payload, CancellationToken ct = default)
    {
        await _hubContext.Clients.Group(HubGroupNames.Role(roleName)).SendAsync(SignalRHubMethodNames.SecurityEvents.ForceLogout, payload, ct);
        _logger.LogInformation("Sent security state change to role {RoleName}", roleName);
    }
}
