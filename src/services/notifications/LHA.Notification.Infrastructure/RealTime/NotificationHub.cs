using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public async Task SubscribeToUser(string tenantId, string userId)
    {
        var groupName = $"{tenantId}:{userId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} subscribed to notifications in tenant {TenantId}", userId, tenantId);
    }

    public async Task UnsubscribeFromUser(string tenantId, string userId)
    {
        var groupName = $"{tenantId}:{userId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} unsubscribed from notifications in tenant {TenantId}", userId, tenantId);
    }

    public async Task AcknowledgeNotification(string notificationId)
    {
        _logger.LogInformation("Notification {NotificationId} acknowledged by connection {ConnectionId}", notificationId, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        _logger.LogInformation("User {UserId} disconnected from notification hub", userId);
        await base.OnDisconnectedAsync(exception);
    }
}
