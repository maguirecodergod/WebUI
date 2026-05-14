using LHA.Ddd.Domain;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record NotificationDeliveredDomainEvent(
    Guid NotificationId,
    Guid? TenantId,
    Guid RecipientId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}