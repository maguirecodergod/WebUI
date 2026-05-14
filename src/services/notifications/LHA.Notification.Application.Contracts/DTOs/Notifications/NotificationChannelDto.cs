using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record NotificationChannelDto(
    CNotificationChannel Channel,
    CDeliveryStatus Status,
    CProviderType ProviderType,
    string? ExternalId,
    DateTimeOffset? SentAt,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset? FailedAt,
    string? FailureReason,
    int RetryCount,
    Dictionary<string, string> Metadata);