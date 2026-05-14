using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record TemplatePreviewDto(
    Guid TemplateId,
    string Code,
    CNotificationType Type,
    string Locale,
    CNotificationChannel Channel,
    Dictionary<string, object> Variables,
    CNotificationPriority Priority);