using LHA.EventBus;

namespace LHA.Mega.Domain.Shared.Events;

/// <summary>
/// Published when a MegaAccount is deleted.
/// Consumed by Account service for cleanup/sync.
/// </summary>
[EventName("mega.account.deleted")]
[EventVersion(1)]
public sealed record MegaAccountDeletedEvent : IntegrationEvent
{
    public required Guid AccountId { get; init; }
}
