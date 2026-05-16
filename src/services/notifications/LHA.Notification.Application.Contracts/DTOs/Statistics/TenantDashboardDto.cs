namespace LHA.Notification.Application.Contracts;

public record TenantDashboardDto(
    Guid TenantId,
    NotificationStatsDto GlobalStats,
    List<ChannelStatsDto> ChannelStats,
    long ActiveNotifications,
    long CompletedBatches,
    long FailedBatches);
