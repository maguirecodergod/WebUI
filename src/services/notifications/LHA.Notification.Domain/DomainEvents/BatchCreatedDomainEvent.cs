using LHA.Ddd.Domain;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record BatchCreatedDomainEvent(
    Guid BatchId,
    Guid? TenantId,
    string Name,
    long TotalCount) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}