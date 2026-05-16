using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.DomainEvents;

public sealed record NotificationCreatedDomainEvent(
    Guid NotificationId,
    Guid? TenantId,
    Guid RecipientId,
    CNotificationType Type,
    CNotificationPriority Priority,
    string? Subject,
    string Body,
    Guid? TemplateId = null,
    Dictionary<string, object>? TemplateVariables = null,
    string? ActionUrl = null,
    Dictionary<string, string>? Data = null) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
