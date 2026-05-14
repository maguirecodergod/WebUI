using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IChannelProviderFactory
{
    INotificationChannelProvider CreateProvider(CNotificationChannel channel, CProviderType providerType);
    IEnumerable<CProviderType> GetAvailableProviders(CNotificationChannel channel);
    bool IsProviderAvailable(CNotificationChannel channel, CProviderType providerType);
    IEnumerable<CNotificationChannel> GetSupportedChannels();
}