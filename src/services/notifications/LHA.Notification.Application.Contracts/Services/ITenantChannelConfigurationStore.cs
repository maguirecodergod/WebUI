using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface ITenantChannelConfigurationStore
{
    Task<ChannelConfiguration> GetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
    Task SetConfigurationAsync(Guid? tenantId, CNotificationChannel channel, ChannelConfiguration configuration, CancellationToken cancellationToken = default);
    Task<IEnumerable<CNotificationChannel>> GetSupportedChannelsAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsChannelEnabledAsync(Guid? tenantId, CNotificationChannel channel, CancellationToken cancellationToken = default);
}

public record ChannelConfiguration(
    CProviderType ProviderType,
    string? ServiceAccountJson = null,
    string? ApiKey = null,
    string? Host = null,
    int? Port = null,
    string? Username = null,
    string? Password = null,
    bool UseSsl = true,
    Dictionary<string, string>? CustomSettings = null);