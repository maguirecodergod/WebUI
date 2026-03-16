using LHA.EventBus;
using LHA.Mega.Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Consumer.EventHandlers;

/// <summary>
/// Handles <see cref="MegaAccountCreatedEvent"/> from Mega service.
/// Example: audit logging, syncing account data to Account's read model, etc.
/// </summary>
public sealed class MegaAccountCreatedEventHandler(
    ILogger<MegaAccountCreatedEventHandler> logger) : IEventHandler<MegaAccountCreatedEvent>
{
    public Task HandleAsync(MegaAccountCreatedEvent @event, EventContext context, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[AccountConsumer] Received MegaAccountCreated: Id={AccountId}, Code={Code}, Name={Name}, Tenant={TenantId}",
            @event.AccountId, @event.Code, @event.Name, @event.TenantId);

        // TODO: Implement business logic
        // Examples:
        // - Create a read model/projection in Account's database
        // - Send notification to administrators
        // - Trigger downstream workflows

        return Task.CompletedTask;
    }
}
