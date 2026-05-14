using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.Repositories;

public interface ITenantChannelConfigurationRepository : IRepository<TenantChannelConfigurationEntity, Guid>
{
    Task<TenantChannelConfigurationEntity?> GetAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
}
