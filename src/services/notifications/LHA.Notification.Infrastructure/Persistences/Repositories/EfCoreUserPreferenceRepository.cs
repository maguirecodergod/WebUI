using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

/// <summary>
/// EF Core (MongoDB) implementation of <see cref="IUserPreferenceRepository"/>.
/// </summary>
public sealed class EfCoreUserPreferenceRepository
    : EfCoreRepository<NotificationDbContext, UserPreferenceEntity, Guid>, IUserPreferenceRepository
{
    public EfCoreUserPreferenceRepository(IDbContextProvider<NotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<UserPreferenceEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserPreferenceEntity>> GetByOptOutStatusAsync(bool optOut, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(p => p.GlobalOptOut == optOut)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<UserPreferenceEntity> GetByTenantCursorAsync(
        Guid? tenantId, int batchSize, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var lastId = Guid.Empty;

        while (true)
        {
            var batch = await dbSet
                .Where(p => p.TenantId == tenantId && p.Id.CompareTo(lastId) > 0)
                .OrderBy(p => p.Id)
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
    public async Task<bool> IsUserOptedOutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(p => p.UserId == userId)
            .Select(p => p.GlobalOptOut)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsChannelEnabledAsync(Guid userId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(p => p.UserId == userId)
            .SelectMany(p => p.Channels)
            .AnyAsync(c => c.Channel == channel && c.Enabled, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CNotificationChannel>> GetEnabledChannelsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var preference = await dbSet
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return preference?.Channels
            .Where(c => c.Enabled)
            .Select(c => c.Channel)
            ?? [];
    }
}
