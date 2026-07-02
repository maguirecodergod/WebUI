using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using LHA.Core.Security;
using Microsoft.AspNetCore.Authorization;

namespace LHA.Notification.Infrastructure;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public async Task SubscribeToUser(string tenantId, string userId)
    {
        var authenticatedUserId = Context.User?.FindFirstValue(LhaClaimTypes.Subject)
            ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.UserIdentifier;

        if (!string.Equals(authenticatedUserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            throw new HubException("Cannot subscribe to another user's group.");
        }

        var groupName = HubGroupNames.User(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} subscribed to notifications in tenant {TenantId}", userId, tenantId);
    }

    public async Task UnsubscribeFromUser(string tenantId, string userId)
    {
        var groupName = HubGroupNames.User(userId);
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

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(LhaClaimTypes.Subject)
            ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.UserIdentifier;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, HubGroupNames.User(userId));
        }

        var roles = Context.User?.FindAll(LhaClaimTypes.Role)
            .Concat(Context.User.FindAll(ClaimTypes.Role))
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

        foreach (var role in roles)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, HubGroupNames.Role(role));
        }

        await base.OnConnectedAsync();
    }
}

internal static class HubGroupNames
{
    public static string User(string userId) => $"Group_User_{userId}";

    public static string Role(string roleName) => $"Group_Role_{roleName}";
}
