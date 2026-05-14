using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record NotificationSummaryDto(
    Guid Id,
    Guid RecipientId,
    CRecipientType RecipientType,
    CNotificationType Type,
    CNotificationPriority Priority,
    CDeliveryStatus Status,
    string? Subject,
    DateTimeOffset CreatedAt);