using LHA.Ddd.Domain;

namespace LHA.Notification.Domain.Repositories;

public interface INotificationRepository : IRepository<NotificationEntity, Guid>
{
    Task<IEnumerable<NotificationEntity>> GetPendingByTenantAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetQueuedByTenantAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetExpiredByTenantAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationEntity>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default);
    Task<NotificationEntity?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<NotificationEntity> GetByRecipientCursorAsync(Guid recipientId, int batchSize, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByRecipientAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default);
}
