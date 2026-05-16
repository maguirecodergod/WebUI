using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.ValueObjects;

public sealed class NotificationChannelEntity
{
    public CNotificationChannel Channel { get; private set; }
    public CDeliveryStatus Status { get; private set; }
    public CProviderType ProviderType { get; private set; }
    public string? ExternalId { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    public NotificationChannelEntity(
        CNotificationChannel channel,
        CProviderType providerType,
        string? externalId = null)
    {
        Channel = channel;
        ProviderType = providerType;
        ExternalId = externalId;
        Status = CDeliveryStatus.Queued;
    }

    public void MarkAsSent(DateTimeOffset sentAt)
    {
        if (Status != CDeliveryStatus.Sent)
        {
            Status = CDeliveryStatus.Sent;
            SentAt = sentAt;
        }
    }

    public void MarkAsDelivered(DateTimeOffset deliveredAt)
    {
        if (Status != CDeliveryStatus.Delivered)
        {
            Status = CDeliveryStatus.Delivered;
            DeliveredAt = deliveredAt;
        }
    }

    public void MarkAsFailed(DateTimeOffset failedAt, string reason)
    {
        Status = CDeliveryStatus.Failed;
        FailedAt = failedAt;
        FailureReason = reason;
        RetryCount++;
    }

    public void SetMetadata(string key, string value)
    {
        Metadata[key] = value;
    }
}
