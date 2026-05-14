using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record BatchProgressDto(
    Guid BatchId,
    Guid TenantId,
    string Name,
    CBatchStatus Status,
    long TotalCount,
    long SentCount,
    long DeliveredCount,
    long FailedCount,
    long PendingCount,
    double ProgressPercentage,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    TimeSpan? EstimatedTimeRemaining);