using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

/// <summary>
/// EF Core (MongoDB) implementation of <see cref="INotificationRepository"/>.
/// </summary>
public sealed class EfCoreNotificationRepository
    : EfCoreRepository<NotificationDbContext, NotificationEntity, Guid>, INotificationRepository
{
    public EfCoreNotificationRepository(IDbContextProvider<NotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationEntity>> GetPendingByTenantAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.Status == CDeliveryStatus.Pending)
            .OrderBy(n => n.CreationTime)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationEntity>> GetQueuedByTenantAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.Status == CDeliveryStatus.Queued)
            .OrderBy(n => n.CreationTime)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationEntity>> GetExpiredByTenantAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.ExpiresAt != null && n.ExpiresAt < now)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationEntity>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.CorrelationId == correlationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationEntity>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.Tags.Any(t => tags.Contains(t)))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NotificationEntity?> GetByBatchIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(n => n.BatchId == batchId, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<NotificationEntity> GetByRecipientCursorAsync(
        Guid recipientId, int batchSize, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var lastId = Guid.Empty;

        while (true)
        {
            var batch = await dbSet
                .Where(n => n.RecipientId == recipientId && n.Id.CompareTo(lastId) > 0)
                .OrderBy(n => n.Id)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (batch.Count == 0)
                yield break;

            foreach (var item in batch)
            {
                yield return item;
            }

            lastId = batch[^1].Id;
        }
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.TenantId == tenantId 
                && n.Status != CDeliveryStatus.Read 
                && n.Status != CDeliveryStatus.Cancelled)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountByRecipientAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.TenantId == tenantId
                && n.RecipientId == recipientId
                && n.Status != CDeliveryStatus.Read
                && n.Status != CDeliveryStatus.Cancelled)
            .CountAsync(cancellationToken);
    }
}
