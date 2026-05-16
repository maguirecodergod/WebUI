using LHA.Ddd.Domain;
namespace LHA.Notification.Domain.DomainEvents;

public sealed record BatchCompletedDomainEvent(
    Guid BatchId,
    Guid? TenantId,
    string Name,
    long TotalCount,
    long DeliveredCount,
    long FailedCount) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
