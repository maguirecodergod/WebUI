using LHA.Notification.Domain.Shared;
namespace LHA.Notification.Application.Contracts;

public record NotificationRetryMessage(
    Guid NotificationId,
    Guid TenantId,
    Guid RecipientId,
    CNotificationChannel Channel,
    int RetryCount,
    string FailureReason,
    DateTimeOffset CreatedAt);
