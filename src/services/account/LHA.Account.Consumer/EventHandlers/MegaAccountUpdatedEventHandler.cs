using LHA.EventBus;
using LHA.Mega.Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Consumer.EventHandlers;

/// <summary>
/// Handles <see cref="MegaAccountUpdatedEvent"/> from Mega service.
/// </summary>
public sealed class MegaAccountUpdatedEventHandler(
    ILogger<MegaAccountUpdatedEventHandler> logger) : IEventHandler<MegaAccountUpdatedEvent>
{
    public Task HandleAsync(MegaAccountUpdatedEvent @event, EventContext context, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[AccountConsumer] Received MegaAccountUpdated: Id={AccountId}, Name={Name}, Active={IsActive}, Tenant={TenantId}",
            @event.AccountId, @event.Name, @event.IsActive, @event.TenantId);

        // TODO: Implement business logic
        // Examples:
        // - Update read model/projection
        // - Deactivate user accounts if IsActive=false

        return Task.CompletedTask;
    }
}
