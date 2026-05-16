using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Notification.Domain.DomainEvents;
using LHA.Notification.Domain.Shared;
using LHA.Notification.Domain.ValueObjects;

namespace LHA.Notification.Domain;

public sealed class NotificationEntity : FullAuditedAggregateRoot<Guid>,
    IMultiTenant
{
    public string CorrelationId { get; private set; } = default!;
    public Guid? BatchId { get; private set; }
    public Guid RecipientId { get; private set; } = default!;
    public CRecipientType RecipientType { get; private set; }
    public CNotificationType Type { get; private set; }
    public CNotificationPriority Priority { get; private set; }
    public CDeliveryStatus Status { get; private set; }
    public string? Subject { get; private set; }
    public string Body { get; private set; } = default!;
    public Dictionary<string, string> Data { get; private set; } = new();
    public string? ImageUrl { get; private set; }
    public string? ActionUrl { get; private set; }
    public Guid? TemplateId { get; private set; }
    public Dictionary<string, object> TemplateVariables { get; private set; } = new();
    public DateTimeOffset? ScheduledAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public int RetryCount { get; private set; }
    public int MaxRetries { get; private set; }
    public List<NotificationChannelEntity> Channels { get; private set; } = new();
    public List<string> Tags { get; private set; } = new();

    public Guid? TenantId { get; private set; }


    private NotificationEntity()
    {
    }

    public NotificationEntity(
        string correlationId,
        Guid recipientId,
        CRecipientType recipientType,
        CNotificationType type,
        CNotificationPriority priority,
        string? subject,
        string body,
        string? imageUrl = null,
        string? actionUrl = null,
        Guid? templateId = null,
        Dictionary<string, object>? templateVariables = null,
        Dictionary<string, string>? data = null)
    {
        CorrelationId = correlationId;
        RecipientId = recipientId;
        RecipientType = recipientType;
        Type = type;
        Priority = priority;
        Subject = subject;
        Body = body;
        ImageUrl = imageUrl;
        ActionUrl = actionUrl;
        TemplateId = templateId;
        TemplateVariables = templateVariables ?? new();
        Data = data ?? new();
        Status = CDeliveryStatus.Pending;
        MaxRetries = NotificationConstants.MaxRetries;

        AddDomainEvent(new NotificationCreatedDomainEvent(Id, TenantId, RecipientId, Type, Priority, Subject, Body, TemplateId, TemplateVariables, ActionUrl, Data));
    }

    public void AddChannel(NotificationChannelEntity channel)
    {
        if (Channels.All(c => c.Channel != channel.Channel))
        {
            Channels.Add(channel);
        }
    }

    public void MarkAsSent(DateTimeOffset sentAt)
    {
        if (Status != CDeliveryStatus.Sent)
        {
            Status = CDeliveryStatus.Sent;
            SentAt = sentAt;
            AddDomainEvent(new NotificationSentDomainEvent(Id, TenantId, RecipientId));
        }
    }

    public void MarkAsDelivered(DateTimeOffset deliveredAt, NotificationChannelEntity channel)
    {
        var channelEntity = Channels.FirstOrDefault(c => c.Channel == channel.Channel);
        if (channelEntity != null)
        {
            channelEntity.MarkAsDelivered(deliveredAt);
            if (!Channels.Any(c => c.Status != CDeliveryStatus.Delivered))
            {
                Status = CDeliveryStatus.Delivered;
                DeliveredAt = deliveredAt;
                AddDomainEvent(new NotificationDeliveredDomainEvent(Id, TenantId, RecipientId));
            }
        }
    }

    public void MarkAsRead(DateTimeOffset readAt)
    {
        if (Status != CDeliveryStatus.Read)
        {
            Status = CDeliveryStatus.Read;
            ReadAt = readAt;
            AddDomainEvent(new NotificationReadDomainEvent(Id, TenantId, RecipientId));
        }
    }

    public void MarkAsFailed(DateTimeOffset failedAt, NotificationChannelEntity channel, string reason)
    {
        channel.MarkAsFailed(failedAt, reason);
        RetryCount++;

        if (RetryCount >= MaxRetries)
        {
            Status = CDeliveryStatus.Failed;
            FailedAt = failedAt;
            AddDomainEvent(new NotificationFailedDomainEvent(Id, TenantId, RecipientId, reason));
        }
        else
        {
            Status = CDeliveryStatus.Pending;
            AddDomainEvent(new NotificationFailedDomainEvent(Id, TenantId, RecipientId, reason));
        }
    }

    public void Cancel()
    {
        if (Status == CDeliveryStatus.Pending)
        {
            Status = CDeliveryStatus.Cancelled;
        }
    }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow;
    }

    public void ScheduleFor(DateTimeOffset scheduledAt)
    {
        ScheduledAt = scheduledAt;
        Status = CDeliveryStatus.Queued;
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }

    public void SetTemplate(Guid templateId, Dictionary<string, object> variables)
    {
        TemplateId = templateId;
        TemplateVariables = variables;
    }

    public void SetExpiresAt(DateTimeOffset expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    public void UpdateContent(string? subject, string body)
    {
        Subject = subject;
        Body = body;
    }
}
