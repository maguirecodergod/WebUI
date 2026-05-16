using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IChannelConfigurationStore
{
    Task<ChannelConfigurationDto> GetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
    Task SetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, ChannelConfigurationDto configuration, CancellationToken cancellationToken = default);
    Task<IEnumerable<CNotificationChannel>> GetSupportedChannelsAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsChannelEnabledAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
}
