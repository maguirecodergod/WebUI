using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record NotificationDto(
    Guid Id,
    Guid TenantId,
    Guid CorrelationId,
    Guid? BatchId,
    Guid RecipientId,
    CRecipientType RecipientType,
    CNotificationType Type,
    CNotificationPriority Priority,
    CDeliveryStatus Status,
    string? Subject,
    string Body,
    Dictionary<string, string> Data,
    string? ImageUrl,
    string? ActionUrl,
    string? TemplateId,
    Dictionary<string, object> TemplateVariables,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? SentAt,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset? ReadAt,
    DateTimeOffset? FailedAt,
    int RetryCount,
    int MaxRetries,
    List<NotificationChannelDto> Channels,
    List<string> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
