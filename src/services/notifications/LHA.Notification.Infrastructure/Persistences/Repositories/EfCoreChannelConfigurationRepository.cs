using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

public sealed class EfCoreChannelConfigurationRepository : EfCoreRepository<NotificationDbContext, ChannelConfigurationEntity, Guid>, IChannelConfigurationRepository
{
    public EfCoreChannelConfigurationRepository(IDbContextProvider<NotificationDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<ChannelConfigurationEntity?> GetAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Set<ChannelConfigurationEntity>()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Channel == channel, cancellationToken);
    }

    public async Task<List<ChannelConfigurationEntity>> GetListAsync(Guid? tenantId, bool? isEnabled = null, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Set<ChannelConfigurationEntity>()
            .Where(x => x.TenantId == tenantId);

        if (isEnabled.HasValue)
        {
            query = query.Where(x => x.IsEnabled == isEnabled.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Set<ChannelConfigurationEntity>()
            .AnyAsync(x => x.TenantId == tenantId && x.Channel == channel, cancellationToken);
    }
}
