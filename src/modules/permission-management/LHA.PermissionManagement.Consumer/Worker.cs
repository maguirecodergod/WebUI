using LHA.EventBus;
using LHA.PermissionManagement.Application.Contracts;

namespace LHA.PermissionManagement.Consumer;

public sealed class PermissionGrantedEventHandler(
    ILogger<PermissionGrantedEventHandler> logger)
    : IEventHandler<PermissionGrantedEto>
{
    public Task HandleAsync(PermissionGrantedEto @event, EventContext context,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Permission granted: {PermissionName} → {ProviderName}:{ProviderKey} (Tenant={TenantId})",
            @event.PermissionName, @event.ProviderName, @event.ProviderKey, @event.TenantId);

        // TODO: Invalidate permission caches, notify other services, etc.
        return Task.CompletedTask;
    }
}

public sealed class PermissionRevokedEventHandler(
    ILogger<PermissionRevokedEventHandler> logger)
    : IEventHandler<PermissionRevokedEto>
{
    public Task HandleAsync(PermissionRevokedEto @event, EventContext context,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Permission revoked: {PermissionName} → {ProviderName}:{ProviderKey} (Tenant={TenantId})",
            @event.PermissionName, @event.ProviderName, @event.ProviderKey, @event.TenantId);

        // TODO: Invalidate permission caches, notify other services, etc.
        return Task.CompletedTask;
    }
}
