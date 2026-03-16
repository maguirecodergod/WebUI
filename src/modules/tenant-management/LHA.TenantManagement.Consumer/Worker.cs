using LHA.EventBus;
using LHA.TenantManagement.Application.Contracts;

namespace LHA.TenantManagement.Consumer;

/// <summary>
/// Handles <see cref="TenantCreatedEto"/> integration events.
/// For example: provisioning per-tenant databases, sending welcome notifications, etc.
/// </summary>
public sealed class TenantCreatedEventHandler(ILogger<TenantCreatedEventHandler> logger)
    : IEventHandler<TenantCreatedEto>
{
    public Task HandleAsync(TenantCreatedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant created: {TenantId} — Name: {Name}, CreatedAt: {CreationTime}",
            @event.TenantId, @event.Name, @event.CreationTime);

        // TODO: Provision per-tenant database, send welcome email, etc.
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles <see cref="TenantActivationChangedEto"/> integration events.
/// </summary>
public sealed class TenantActivationChangedEventHandler(ILogger<TenantActivationChangedEventHandler> logger)
    : IEventHandler<TenantActivationChangedEto>
{
    public Task HandleAsync(TenantActivationChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant {TenantId} activation changed: IsActive={IsActive}",
            @event.TenantId, @event.IsActive);

        // TODO: Invalidate caches, suspend background jobs, etc.
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles <see cref="TenantConnectionStringChangedEto"/> integration events.
/// </summary>
public sealed class TenantConnectionStringChangedEventHandler(ILogger<TenantConnectionStringChangedEventHandler> logger)
    : IEventHandler<TenantConnectionStringChangedEto>
{
    public Task HandleAsync(TenantConnectionStringChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant {TenantId} connection string changed: {Name}",
            @event.TenantId, @event.ConnectionStringName);

        // TODO: Rotate connection pools, update tenant store cache, etc.
        return Task.CompletedTask;
    }
}
