using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface INotificationService
{
    Task<NotificationDto> SendAsync(SendNotificationDto request, CancellationToken cancellationToken = default);
    Task<NotificationDto> ScheduleAsync(ScheduleNotificationDto request, CancellationToken cancellationToken = default);
    Task<NotificationPagedResultDto<NotificationDto>> GetByRecipientAsync(Guid recipientId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> MarkAsReadAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid recipientId, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<NotificationPagedResultDto<NotificationSummaryDto>> SearchAsync(Guid recipientId, string? query, CNotificationType? type, CDeliveryStatus? status, DateTimeOffset? from, DateTimeOffset? to, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}