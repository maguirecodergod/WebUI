using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record SendNotificationDto(
    Guid RecipientId,
    CRecipientType RecipientType,
    CNotificationType Type,
    CNotificationPriority Priority,
    string? Subject,
    string Body,
    Dictionary<string, string>? Data,
    string? ImageUrl,
    string? ActionUrl,
    Guid? TemplateId,
    Dictionary<string, object>? TemplateVariables,
    List<string>? Tags,
    DateTimeOffset? ExpiresAt,
    List<CNotificationChannel>? Channels,
    bool SkipRateLimit);