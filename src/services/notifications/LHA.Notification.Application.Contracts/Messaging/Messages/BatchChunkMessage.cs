using LHA.Notification.Domain.Shared;
namespace LHA.Notification.Application.Contracts;

public record BatchChunkMessage(
    string BatchId,
    string TenantId,
    List<BatchRecipientMessage> Recipients,
    CNotificationChannel PrimaryChannel,
    Guid? TemplateId,
    Dictionary<string, object>? TemplateVariables,
    DateTimeOffset CreatedAt);

public record BatchRecipientMessage(
    string RecipientId,
    CRecipientType RecipientType,
    Dictionary<string, string> Data,
    Dictionary<string, object>? Variables,
    string? CorrelationId);