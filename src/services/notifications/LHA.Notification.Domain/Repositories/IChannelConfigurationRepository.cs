using LHA.Ddd.Domain;
using LHA.Shared.Domain.Enums.Notification;

namespace LHA.Notification.Domain.Repositories;

public interface IChannelConfigurationRepository : IRepository<ChannelConfigurationEntity, Guid>
{
    Task<ChannelConfigurationEntity?> GetAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
    
    Task<List<ChannelConfigurationEntity>> GetListAsync(Guid? tenantId, bool? isEnabled = null, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
}
