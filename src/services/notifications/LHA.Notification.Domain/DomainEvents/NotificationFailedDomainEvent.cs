using LHA.Ddd.Domain;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record NotificationFailedDomainEvent(
    Guid NotificationId,
    Guid? TenantId,
    Guid RecipientId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
