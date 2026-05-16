using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

/// <summary>
/// EF Core (MongoDB) implementation of <see cref="INotificationBatchRepository"/>.
/// </summary>
public sealed class EfCoreNotificationBatchRepository
    : EfCoreRepository<NotificationDbContext, NotificationBatchEntity, Guid>, INotificationBatchRepository
{
    public EfCoreNotificationBatchRepository(IDbContextProvider<NotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<NotificationBatchEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(b => b.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationBatchEntity>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(b => b.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationBatchEntity>> GetProcessingAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(b => b.Status == CBatchStatus.Processing)
            .OrderBy(b => b.StartedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationBatchEntity>> GetScheduledAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(b => b.Status == CBatchStatus.Scheduled)
            .OrderBy(b => b.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationBatchEntity>> GetByStatusAsync(CBatchStatus status, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(b => b.Status == status)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<NotificationBatchEntity> GetByTenantCursorAsync(
        Guid? tenantId, int batchSize, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var lastId = Guid.Empty;

        while (true)
        {
            var batch = await dbSet
                .Where(b => b.TenantId == tenantId && b.Id.CompareTo(lastId) > 0)
                .OrderBy(b => b.Id)
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
}
