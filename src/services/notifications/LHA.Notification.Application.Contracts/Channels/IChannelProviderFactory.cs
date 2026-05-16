using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IChannelProviderFactory
{
    INotificationChannelProvider CreateProvider(CNotificationChannel channel, CProviderType providerType, Guid tenantId);
    Task<IEnumerable<CProviderType>> GetAvailableProviders(CNotificationChannel channel, Guid tenantId);
    Task<bool> IsProviderAvailable(CNotificationChannel channel, CProviderType providerType, Guid tenantId);
    Task<IEnumerable<CNotificationChannel>> GetSupportedChannels(Guid tenantId);
}
