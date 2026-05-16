using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record BatchDto(
    Guid Id,
    Guid TenantId,
    string Name,
    CBatchStatus Status,
    long TotalCount,
    long SentCount,
    long DeliveredCount,
    long FailedCount,
    long PendingCount,
    Guid? TemplateId,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
