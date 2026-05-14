using LHA.EventBus;
using LHA.TenantManagement.Application.Contracts;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.Logging;

namespace LHA.TenantManagement.Application.DomainEventHandlers;

public sealed class TenantCreatedDomainEventHandler(
    IEventBus eventBus,
    ILogger<TenantCreatedDomainEventHandler> logger)
    : IEventHandler<TenantCreatedDomainEvent>
{
    public async Task HandleAsync(TenantCreatedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Domain Event: Tenant {TenantId} ({Name}) created.", @event.TenantId, @event.Name);

        await eventBus.PublishAsync(
            new TenantCreatedEto(@event.TenantId, @event.Name, DateTimeOffset.UtcNow),
            cancellationToken);
    }
}

public sealed class TenantNameChangedDomainEventHandler(
    IEventBus eventBus,
    ILogger<TenantNameChangedDomainEventHandler> logger)
    : IEventHandler<TenantNameChangedDomainEvent>
{
    public async Task HandleAsync(TenantNameChangedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Domain Event: Tenant {TenantId} name changed from {OldName} to {NewName}.", 
            @event.TenantId, @event.OldName, @event.NewName);

        await eventBus.PublishAsync(
            new TenantNameChangedEto(@event.TenantId, @event.OldName, @event.NewName),
            cancellationToken);
    }
}

public sealed class TenantActivationChangedDomainEventHandler(
    IEventBus eventBus,
    ILogger<TenantActivationChangedDomainEventHandler> logger)
    : IEventHandler<TenantActivationChangedDomainEvent>
{
    public async Task HandleAsync(TenantActivationChangedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Domain Event: Tenant {TenantId} activation status changed to {IsActive}.", 
            @event.TenantId, @event.IsActive);

        await eventBus.PublishAsync(
            new TenantActivationChangedEto(@event.TenantId, @event.IsActive),
            cancellationToken);
    }
}

public sealed class TenantConnectionStringChangedDomainEventHandler(
    IEventBus eventBus,
    ILogger<TenantConnectionStringChangedDomainEventHandler> logger)
    : IEventHandler<TenantConnectionStringChangedDomainEvent>
{
    public async Task HandleAsync(TenantConnectionStringChangedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Domain Event: Tenant {TenantId} connection string {ConnectionStringName} changed.", 
            @event.TenantId, @event.ConnectionStringName);

        await eventBus.PublishAsync(
            new TenantConnectionStringChangedEto(@event.TenantId, @event.ConnectionStringName),
            cancellationToken);
    }
}
