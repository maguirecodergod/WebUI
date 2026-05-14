using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Statistics;

/// <summary>
/// Application service for notification statistics.
/// </summary>
public sealed class NotificationStatsAppService : ApplicationService, INotificationStatsService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationBatchRepository _batchRepository;

    public NotificationStatsAppService(
        INotificationRepository notificationRepository,
        INotificationBatchRepository batchRepository)
    {
        _notificationRepository = notificationRepository;
        _batchRepository = batchRepository;
    }

    /// <inheritdoc />
    public async Task<NotificationStatsDto> GetGlobalStatsAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation. In a real system, you'd use a more efficient query or a pre-calculated table.
        var all = await _notificationRepository.GetListAsync(cancellationToken);
        var notifications = all.Where(n => n.CreationTime >= from && n.CreationTime <= to).ToList();

        return CalculateStats(tenantId, from, to, notifications);
    }

    /// <inheritdoc />
    public async Task<NotificationStatsDto> GetUserStatsAsync(Guid userId, Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var all = await _notificationRepository.GetListAsync(cancellationToken);
        var notifications = all.Where(n => n.RecipientId == userId && n.CreationTime >= from && n.CreationTime <= to).ToList();

        return CalculateStats(tenantId, from, to, notifications);
    }

    /// <inheritdoc />
    public async Task<List<ChannelStatsDto>> GetChannelStatsAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var all = await _notificationRepository.GetListAsync(cancellationToken);
        var notifications = all.Where(n => n.CreationTime >= from && n.CreationTime <= to).ToList();

        var result = new List<ChannelStatsDto>();
        foreach (var channel in Enum.GetValues<CNotificationChannel>())
        {
            var channelNotifications = notifications.Where(n => n.Channels.Any(c => c.Channel == channel)).ToList();
            if (!channelNotifications.Any()) continue;

            var sent = channelNotifications.Count(n => n.Channels.Any(c => c.Channel == channel && c.Status >= CDeliveryStatus.Sent));
            var delivered = channelNotifications.Count(n => n.Channels.Any(c => c.Channel == channel && c.Status == CDeliveryStatus.Delivered));
            var read = channelNotifications.Count(n => n.Channels.Any(c => c.Channel == channel && c.Status == CDeliveryStatus.Read));
            var failed = channelNotifications.Count(n => n.Channels.Any(c => c.Channel == channel && c.Status == CDeliveryStatus.Failed));

            result.Add(new ChannelStatsDto(
                Channel: channel,
                TotalSent: sent,
                TotalDelivered: delivered,
                TotalRead: read,
                TotalFailed: failed,
                DeliveryRate: sent > 0 ? (double)delivered / sent * 100 : 0,
                ReadRate: delivered > 0 ? (double)read / delivered * 100 : 0));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<NotificationStatsDto> GetNotificationTypeStatsAsync(Guid tenantId, CNotificationType type, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var all = await _notificationRepository.GetListAsync(cancellationToken);
        var notifications = all.Where(n => n.Type == type && n.CreationTime >= from && n.CreationTime <= to).ToList();

        return CalculateStats(tenantId, from, to, notifications);
    }

    /// <inheritdoc />
    public async Task<TenantDashboardDto> GetDashboardAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var monthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);

        var globalStats = await GetGlobalStatsAsync(tenantId, monthStart, now, cancellationToken);
        var channelStats = await GetChannelStatsAsync(tenantId, monthStart, now, cancellationToken);

        var activeNotifications = (await _notificationRepository.GetPendingByTenantAsync(cancellationToken)).Count() +
                                   (await _notificationRepository.GetQueuedByTenantAsync(cancellationToken)).Count();

        var completedBatches = (await _batchRepository.GetByStatusAsync(CBatchStatus.Completed, cancellationToken)).Count();
        var failedBatches = (await _batchRepository.GetByStatusAsync(CBatchStatus.Failed, cancellationToken)).Count();

        return new TenantDashboardDto(
            TenantId: tenantId,
            GlobalStats: globalStats,
            ChannelStats: channelStats,
            ActiveNotifications: activeNotifications,
            CompletedBatches: completedBatches,
            FailedBatches: failedBatches);
    }

    /// <inheritdoc />
    public async Task<List<DeliveryRateDto>> GetDeliveryRatesAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var all = await _notificationRepository.GetListAsync(cancellationToken);
        var notifications = all.Where(n => n.CreationTime >= from && n.CreationTime <= to).ToList();

        var total = notifications.Count;
        if (total == 0) return new List<DeliveryRateDto>();

        return notifications.GroupBy(n => n.Status)
            .Select(g => new DeliveryRateDto(
                Status: g.Key,
                Count: g.Count(),
                Percentage: (double)g.Count() / total * 100))
            .ToList();
    }

    private static NotificationStatsDto CalculateStats(Guid tenantId, DateTimeOffset from, DateTimeOffset to, List<NotificationEntity> notifications)
    {
        var sent = notifications.Count(n => n.Status >= CDeliveryStatus.Sent);
        var delivered = notifications.Count(n => n.Status >= CDeliveryStatus.Delivered);
        var read = notifications.Count(n => n.Status == CDeliveryStatus.Read);
        var failed = notifications.Count(n => n.Status == CDeliveryStatus.Failed);
        var cancelled = notifications.Count(n => n.Status == CDeliveryStatus.Cancelled);

        return new NotificationStatsDto(
            TenantId: tenantId,
            PeriodStart: from,
            PeriodEnd: to,
            TotalSent: sent,
            TotalDelivered: delivered,
            TotalRead: read,
            TotalFailed: failed,
            TotalCancelled: cancelled,
            DeliveryRate: sent > 0 ? (double)delivered / sent * 100 : 0,
            ReadRate: delivered > 0 ? (double)read / delivered * 100 : 0,
            FailureRate: sent > 0 ? (double)failed / sent * 100 : 0);
    }
}
