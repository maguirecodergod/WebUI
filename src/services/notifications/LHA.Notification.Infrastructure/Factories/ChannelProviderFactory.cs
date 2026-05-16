using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Notification.Infrastructure
{
    internal sealed class ChannelProviderFactory : IChannelProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannelConfigurationStore _configStore;

        public ChannelProviderFactory(
            IServiceProvider serviceProvider,
            IChannelConfigurationStore configStore)
        {
            _serviceProvider = serviceProvider;
            _configStore = configStore;
        }

        public INotificationChannelProvider CreateProvider(CNotificationChannel channel, CProviderType providerType, Guid tenantId)
        {
            return providerType switch
            {
                CProviderType.Fcm => _serviceProvider.GetRequiredService<IFcmPushProvider>(),
                CProviderType.Apns => _serviceProvider.GetRequiredService<IApnsPushProvider>(),
                CProviderType.SendGrid => _serviceProvider.GetRequiredService<ISendGridEmailProvider>(),
                CProviderType.AwsSes => _serviceProvider.GetRequiredService<IAwsSesEmailProvider>(),
                CProviderType.Smtp => _serviceProvider.GetRequiredService<ISmtpEmailProvider>(),
                CProviderType.Twilio => _serviceProvider.GetRequiredService<ITwilioSmsProvider>(),
                CProviderType.AwsSns => _serviceProvider.GetRequiredService<IAwsSnsSmsProvider>(),
                CProviderType.WebPush => _serviceProvider.GetRequiredService<IWebPushProvider>(),
                CProviderType.Internal => _serviceProvider.GetRequiredService<IInAppProvider>(),
                _ => throw new NotSupportedException($"Provider type {providerType} is not supported")
            };
        }

        public async Task<IEnumerable<CProviderType>> GetAvailableProviders(CNotificationChannel channel, Guid tenantId)
        {
            var supportChannels = await _configStore.GetSupportedChannelsAsync(tenantId);

            if (supportChannels == null || !supportChannels.Any())
            {
                return Enumerable.Empty<CProviderType>();
            }

            var matchingChannels = supportChannels.Where(c => c == channel).ToList();
            var providers = new List<CProviderType>();

            foreach (var c in matchingChannels)
            {
                var config = await _configStore.GetConfigurationAsync(tenantId, c);
                if (config != null)
                {
                    providers.Add(config.ProviderType);
                }
            }

            return providers.Distinct();
        }

        public async Task<bool> IsProviderAvailable(CNotificationChannel channel, CProviderType providerType, Guid tenantId)
        {
            var available = await GetAvailableProviders(channel, tenantId);
            return available.Contains(providerType);
        }

        public async Task<IEnumerable<CNotificationChannel>> GetSupportedChannels(Guid tenantId)
        {
            return await _configStore.GetSupportedChannelsAsync(tenantId);
        }
    }
}
