using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;


namespace LHA.Notification.Application.Services;

/// <summary>
/// Implementation of <see cref="IChannelConfigurationStore"/>.
/// Implements a fallback mechanism: Tenant -> Global -> Hardcoded Defaults.
/// </summary>
internal sealed class ChannelConfigurationStore : ApplicationService, IChannelConfigurationStore
{
    private readonly IChannelConfigurationRepository _repository;

    public ChannelConfigurationStore(IChannelConfigurationRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<ChannelConfigurationDto> GetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        // 1. Try Tenant-specific configuration
        ChannelConfigurationEntity? configEntity = null;
        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);
        }

        // 2. Fallback to Global configuration (where TenantId is null)
        if (configEntity == null)
        {
            configEntity = await _repository.GetAsync(null, channel, cancellationToken);
        }

        if (configEntity != null)
        {
            return MapToDto(configEntity);
        }

        // 3. Fallback to Hardcoded defaults
        return GetHardcodedDefaults(channel);
    }

    /// <inheritdoc />
    public async Task SetConfigurationAsync(Guid? tenantId,
        CNotificationChannel channel,
        ChannelConfigurationDto configuration,
        CancellationToken cancellationToken = default)
    {
        var configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);
        if (configEntity != null)
        {
            configEntity.Update(
                configuration.ProviderType,
                configuration.Settings);
            await _repository.UpdateAsync(configEntity, cancellationToken);
        }
        else
        {
            configEntity = new ChannelConfigurationEntity(
                tenantId,
                channel,
                configuration.ProviderType,
                configuration.Settings);
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
        // Check tenant specific
        var configEntity = await _repository.GetAsync(tenantId, channel, cancellationToken);

        // If not found, check global
        if (configEntity == null && tenantId.HasValue)
        {
            configEntity = await _repository.GetAsync(null, channel, cancellationToken);
        }

        return configEntity?.IsEnabled ?? true;
    }

    private static ChannelConfigurationDto MapToDto(ChannelConfigurationEntity entity)
    {
        return new ChannelConfigurationDto(
            entity.Id,
            entity.TenantId,
            entity.Channel,
            entity.ProviderType,
            entity.IsEnabled,
            entity.GetSettings<ProviderSettings>());
    }

    private static ChannelConfigurationDto GetHardcodedDefaults(CNotificationChannel channel)
    {
        return channel switch
        {
            CNotificationChannel.Email => new ChannelConfigurationDto(Guid.Empty, null, channel, CProviderType.Smtp, true, new SmtpProviderSettings("localhost", 25)),
            CNotificationChannel.Sms => new ChannelConfigurationDto(Guid.Empty, null, channel, CProviderType.Twilio, true),
            CNotificationChannel.Push => new ChannelConfigurationDto(Guid.Empty, null, channel, CProviderType.Fcm, true),
            _ => new ChannelConfigurationDto(Guid.Empty, null, channel, CProviderType.Internal, true)
        };
    }
}
