using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

/// <summary>
/// EF Core (MongoDB) implementation of <see cref="IDeviceRepository"/>.
/// </summary>
public sealed class EfCoreDeviceRepository
    : EfCoreRepository<NotificationDbContext, DeviceEntity, Guid>, IDeviceRepository
{
    public EfCoreDeviceRepository(IDbContextProvider<NotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeviceEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(d => d.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeviceEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(d => d.TokenHash == tokenHash, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeviceEntity>> GetInactiveDevicesAsync(
        DateTimeOffset lastSeenBefore, int limit, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(d => d.LastSeenAt < lastSeenBefore.DateTime)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeviceEntity>> GetByPlatformAsync(CDevicePlatform platform, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(d => d.Platform == platform)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<DeviceEntity> GetByTenantCursorAsync(
        Guid? tenantId, int batchSize, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var lastId = Guid.Empty;

        while (true)
        {
            var batch = await dbSet
                .Where(d => d.TenantId == tenantId && d.Id.CompareTo(lastId) > 0)
                .OrderBy(d => d.Id)
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
    public async Task<bool> ExistsByUserIdAndPlatformAsync(Guid userId, CDevicePlatform platform, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AnyAsync(d => d.UserId == userId && d.Platform == platform, cancellationToken);
    }
}
