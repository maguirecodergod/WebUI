using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;
public record SendBulkNotificationMessage(
    Guid BatchId,
    Guid TenantId,
    List<BulkNotificationItem> Items,
    Guid? TemplateId,
    Dictionary<string, object> TemplateVariables,
    DateTimeOffset? ExpiresAt,
    CNotificationChannel PrimaryChannel,
    string? CorrelationId,
    DateTimeOffset CreatedAt);

public record BulkNotificationItem(
    Guid RecipientId,
    CRecipientType RecipientType,
    Dictionary<string, string> Data,
    Dictionary<string, object> Variables);