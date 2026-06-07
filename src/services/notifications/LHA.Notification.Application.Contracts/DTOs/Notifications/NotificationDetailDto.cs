using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;


/// <summary>
/// Notification detail DTO.
/// </summary>
/// <param name="Id">Notification ID.</param>
/// <param name="TenantId">Tenant ID.</param>
/// <param name="CorrelationId">Correlation ID.</param>
/// <param name="BatchId">Batch ID.</param>
/// <param name="RecipientId">Recipient ID.</param>
/// <param name="RecipientType">Recipient type.</param>
/// <param name="Type">Notification type.</param>
/// <param name="Priority">Notification priority.</param>
/// <param name="Status">Delivery status.</param>
/// <param name="Subject">Subject.</param>
/// <param name="Body"></param>
/// <param name="Data"></param>
/// <param name="ImageUrl"></param>
/// <param name="ActionUrl"></param>
/// <param name="TemplateId"></param>
/// <param name="TemplateVariables"></param>
/// <param name="ScheduledAt"></param>
/// <param name="ExpiresAt"></param>
/// <param name="SentAt"></param>
/// <param name="DeliveredAt"></param>
/// <param name="ReadAt"></param>
/// <param name="FailedAt"></param>
/// <param name="RetryCount"></param>
/// <param name="MaxRetries"></param>
/// <param name="Channels"></param>
/// <param name="Tags"></param>
/// <param name="CreatedAt"></param>
/// <param name="UpdatedAt"></param>
/// <param name="UnreadCount"></param>
/// <param name="HasNextPage"></param>
public record NotificationDetailDto(
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
    DateTimeOffset UpdatedAt,
    int UnreadCount,
    bool HasNextPage);
