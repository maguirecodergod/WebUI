using LHA.Ddd.Domain;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record TemplateUpdatedDomainEvent(
    Guid TemplateId,
    Guid? TenantId,
    string Code) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}