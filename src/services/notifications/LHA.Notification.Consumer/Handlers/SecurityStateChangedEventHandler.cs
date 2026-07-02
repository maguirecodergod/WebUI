using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Shared.Contracts.Security;

namespace LHA.Notification.Consumer.Handlers;

public sealed class SecurityStateChangedEventHandler(
    INotificationHubContext hubContext,
    ILogger<SecurityStateChangedEventHandler> logger)
    : IEventHandler<SecurityStateChangedEto>
{
    public async Task HandleAsync(
        SecurityStateChangedEto @event,
        EventContext context,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling SecurityStateChanged event: {@Event}", @event);
        var payload = new
        {
            @event.TargetType,
            @event.TargetId,
            @event.VersionUnixSeconds,
            @event.Reason
        };

        if (@event.TargetType == SecurityStateTargetType.User)
        {
            logger.LogInformation("Sending SecurityStateChanged event to user: {UserId}", @event.TargetId);
            await hubContext.SendSecurityStateChangedToUserAsync(@event.TargetId, payload, cancellationToken);
            return;
        }

        logger.LogInformation("Sending SecurityStateChanged event to role: {RoleId}", @event.TargetId);
        await hubContext.SendSecurityStateChangedToRoleAsync(@event.TargetId, payload, cancellationToken);
    }
}

