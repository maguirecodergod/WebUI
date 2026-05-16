using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record DeviceRegisteredDomainEvent(
    Guid DeviceId,
    Guid? TenantId,
    Guid UserId,
    CDevicePlatform Platform) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
