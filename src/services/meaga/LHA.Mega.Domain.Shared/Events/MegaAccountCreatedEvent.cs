using LHA.EventBus;

namespace LHA.Mega.Domain.Shared.Events;

/// <summary>
/// Published when a new MegaAccount is created.
/// Consumed by Account service for audit/sync.
/// </summary>
[EventName("mega.account.created")]
[EventVersion(1)]
public sealed record MegaAccountCreatedEvent : IntegrationEvent
{
    public required Guid AccountId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
}
