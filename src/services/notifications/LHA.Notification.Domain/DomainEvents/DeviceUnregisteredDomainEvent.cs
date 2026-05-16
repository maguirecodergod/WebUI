using LHA.Ddd.Domain;
namespace LHA.Notification.Domain.DomainEvents;

public sealed record DeviceUnregisteredDomainEvent(
    Guid DeviceId,
    Guid? TenantId,
    Guid UserId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
