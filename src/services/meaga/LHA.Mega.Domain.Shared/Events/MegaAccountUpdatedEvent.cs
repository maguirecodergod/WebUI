using LHA.EventBus;

namespace LHA.Mega.Domain.Shared.Events;

/// <summary>
/// Published when a MegaAccount is updated.
/// Consumed by Account service for sync.
/// </summary>
[EventName("mega.account.updated")]
[EventVersion(1)]
public sealed record MegaAccountUpdatedEvent : IntegrationEvent
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public required bool IsActive { get; init; }
}
