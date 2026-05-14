using LHA.EventBus;
using LHA.TenantManagement.Application.Contracts;

namespace LHA.TenantManagement.Consumer;

public sealed class TenantCreatedEventHandler(ILogger<TenantCreatedEventHandler> logger)
    : IEventHandler<TenantCreatedEto>
{
    public Task HandleAsync(TenantCreatedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant created: {TenantId} — Name: {Name}, CreatedAt: {CreationTime}",
            @event.TenantId, @event.Name, @event.CreationTime);

        return Task.CompletedTask;
    }
}

public sealed class TenantNameChangedEventHandler(ILogger<TenantNameChangedEventHandler> logger)
    : IEventHandler<TenantNameChangedEto>
{
    public Task HandleAsync(TenantNameChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant {TenantId} name changed from {OldName} to {NewName}",
            @event.TenantId, @event.OldName, @event.NewName);

        return Task.CompletedTask;
    }
}

public sealed class TenantActivationChangedEventHandler(ILogger<TenantActivationChangedEventHandler> logger)
    : IEventHandler<TenantActivationChangedEto>
{
    public Task HandleAsync(TenantActivationChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant {TenantId} activation changed: IsActive={IsActive}",
            @event.TenantId, @event.IsActive);

        return Task.CompletedTask;
    }
}

public sealed class TenantConnectionStringChangedEventHandler(ILogger<TenantConnectionStringChangedEventHandler> logger)
    : IEventHandler<TenantConnectionStringChangedEto>
{
    public Task HandleAsync(TenantConnectionStringChangedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Tenant {TenantId} connection string changed: {Name}",
            @event.TenantId, @event.ConnectionStringName);

        return Task.CompletedTask;
    }
}
