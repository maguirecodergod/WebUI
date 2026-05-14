using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Services;

/// <summary>
/// Implementation of <see cref="ITenantChannelConfigurationStore"/>.
/// This is a basic implementation that could be extended to read from a database.
/// </summary>
public sealed class TenantChannelConfigurationStore : ApplicationService, ITenantChannelConfigurationStore
{
    private readonly ITenantChannelConfigurationRepository _repository;

    public TenantChannelConfigurationStore(ITenantChannelConfigurationRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<ChannelConfiguration> GetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);
        if (configEntity != null)
        {
            return new ChannelConfiguration(
                configEntity.ProviderType,
                configEntity.ServiceAccountJson,
                configEntity.ApiKey,
                configEntity.Host,
                configEntity.Port,
                configEntity.Username,
                configEntity.Password,
                configEntity.UseSsl,
                configEntity.CustomSettings);
        }

        // Return default configurations if not explicitly set
        return channel switch
        {
            CNotificationChannel.Email => new ChannelConfiguration(CProviderType.Smtp, Host: "localhost", Port: 25),
            CNotificationChannel.Sms => new ChannelConfiguration(CProviderType.Twilio),
            CNotificationChannel.Push => new ChannelConfiguration(CProviderType.Fcm),
            _ => new ChannelConfiguration(CProviderType.Internal)
        };
    }

    /// <inheritdoc />
    public async Task SetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, ChannelConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);
        if (configEntity != null)
        {
            configEntity.Update(
                configuration.ProviderType,
                configuration.ServiceAccountJson,
                configuration.ApiKey,
                configuration.Host,
                configuration.Port,
                configuration.Username,
                configuration.Password,
                configuration.UseSsl,
                configuration.CustomSettings);
            await _repository.UpdateAsync(configEntity, cancellationToken);
        }
        else
        {
            configEntity = new TenantChannelConfigurationEntity(
                tenantId,
                channel,
                configuration.ProviderType,
                configuration.ServiceAccountJson,
                configuration.ApiKey,
                configuration.Host,
                configuration.Port,
                configuration.Username,
                configuration.Password,
                configuration.UseSsl,
                configuration.CustomSettings);
            await _repository.InsertAsync(configEntity, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<CNotificationChannel>> GetSupportedChannelsAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<CNotificationChannel>>(Enum.GetValues<CNotificationChannel>());
    }

    /// <inheritdoc />
    public async Task<bool> IsChannelEnabledAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);
        return configEntity?.IsEnabled ?? true;
    }
}
