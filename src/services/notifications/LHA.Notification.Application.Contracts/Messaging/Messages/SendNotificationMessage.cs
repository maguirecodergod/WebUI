using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record SendNotificationMessage(
    Guid NotificationId,
    Guid TenantId,
    Guid RecipientId,
    CRecipientType RecipientType,
    CNotificationType Type,
    CNotificationPriority Priority,
    string? Subject,
    string Body,
    Dictionary<string, string> Data,
    string? ImageUrl,
    string? ActionUrl,
    Guid? TemplateId,
    Dictionary<string, object> TemplateVariables,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? ExpiresAt,
    List<CNotificationChannel> Channels,
    CNotificationChannel PrimaryChannel,
    string? CorrelationId,
    DateTimeOffset CreatedAt);
