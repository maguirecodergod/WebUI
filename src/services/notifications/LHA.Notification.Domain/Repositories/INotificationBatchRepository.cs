using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.Repositories;

public interface INotificationBatchRepository : IRepository<NotificationBatchEntity, Guid>
{
    Task<NotificationBatchEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationBatchEntity>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationBatchEntity>> GetProcessingAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationBatchEntity>> GetScheduledAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationBatchEntity>> GetByStatusAsync(CBatchStatus status, CancellationToken cancellationToken = default);
    IAsyncEnumerable<NotificationBatchEntity> GetByTenantCursorAsync(Guid? tenantId, int batchSize, CancellationToken cancellationToken = default);
}
