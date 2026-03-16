using LHA.EventBus;
using LHA.Mega.Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Consumer.EventHandlers;

/// <summary>
/// Handles <see cref="MegaAccountDeletedEvent"/> from Mega service.
/// </summary>
public sealed class MegaAccountDeletedEventHandler(
    ILogger<MegaAccountDeletedEventHandler> logger) : IEventHandler<MegaAccountDeletedEvent>
{
    public Task HandleAsync(MegaAccountDeletedEvent @event, EventContext context, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[AccountConsumer] Received MegaAccountDeleted: Id={AccountId}",
            @event.AccountId);

        // TODO: Implement business logic
        // Examples:
        // - Soft-delete synced records
        // - Clean up related data

        return Task.CompletedTask;
    }
}
