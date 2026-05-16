using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain;

public sealed class NotificationBatchEntity : FullAuditedEntity<Guid>,
    IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public CBatchStatus Status { get; private set; }
    public long TotalCount { get; private set; }
    public long SentCount { get; private set; }
    public long DeliveredCount { get; private set; }
    public long FailedCount { get; private set; }
    public long PendingCount { get; private set; }
    public string? TemplateId { get; private set; }
    public DateTimeOffset? ScheduledAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public NotificationBatchEntity()
    {
    }

    public NotificationBatchEntity(
        string name,
        string? templateId = null)
    {
        Name = name;
        TemplateId = templateId;
        Status = CBatchStatus.Draft;
        TotalCount = 0;
        SentCount = 0;
        DeliveredCount = 0;
        FailedCount = 0;
        PendingCount = 0;
    }

    public void SetRecipients(long totalCount)
    {
        TotalCount = totalCount;
        PendingCount = totalCount;
        Status = CBatchStatus.Scheduled;
        ScheduledAt = DateTimeOffset.UtcNow;
    }

    public void StartProcessing()
    {
        if (Status == CBatchStatus.Scheduled)
        {
            Status = CBatchStatus.Processing;
            StartedAt = DateTimeOffset.UtcNow;
        }
    }

    public void MarkAsSent(int count)
    {
        SentCount += count;
        PendingCount -= count;
    }

    public void MarkAsDelivered(int count)
    {
        DeliveredCount += count;
    }

    public void MarkAsFailed(int count)
    {
        FailedCount += count;
        PendingCount -= count;
    }

    public void Complete()
    {
        Status = CBatchStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Fail(string reason)
    {
        Status = CBatchStatus.Failed;
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
