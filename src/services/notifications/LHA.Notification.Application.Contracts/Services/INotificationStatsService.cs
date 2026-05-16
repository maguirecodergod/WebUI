using LHA.Notification.Domain.Shared;
namespace LHA.Notification.Application.Contracts;

public interface INotificationStatsService
{
    Task<NotificationStatsDto> GetGlobalStatsAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<NotificationStatsDto> GetUserStatsAsync(Guid userId, Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<List<ChannelStatsDto>> GetChannelStatsAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<NotificationStatsDto> GetNotificationTypeStatsAsync(Guid tenantId, CNotificationType type, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<TenantDashboardDto> GetDashboardAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<DeliveryRateDto>> GetDeliveryRatesAsync(Guid tenantId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}
