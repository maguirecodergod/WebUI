using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record DeliveryReceiptMessage(
    Guid NotificationId,
    Guid TenantId,
    Guid RecipientId,
    CDeliveryStatus Status,
    string? FailureReason,
    DateTimeOffset ReceivedAt);
