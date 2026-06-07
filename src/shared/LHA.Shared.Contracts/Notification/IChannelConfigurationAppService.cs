using LHA.Ddd.Application;
using LHA.Shared.Domain.Enums.Notification;

namespace LHA.Shared.Contracts.Notification;

/// <summary>
/// Application service for managing notification channel configurations.
/// </summary>
public interface IChannelConfigurationAppService : IApplicationService
{
    /// <summary>
    /// Retrieves a paginated list of channel configurations matching the specified filters.
    /// </summary>
    Task<PagedResultDto<ChannelConfigurationDto>> GetPagedListAsync(GetChannelConfigurationsInput input);

    /// <summary>
    /// Retrieves a single channel configuration by its unique identifier.
    /// </summary>
    Task<ChannelConfigurationDto?> GetAsync(Guid id);

    /// <summary>
    /// Retrieves a channel configuration by notification channel type, optionally scoped to a specific tenant.
    /// </summary>
    Task<ChannelConfigurationDto?> GetByChannelAsync(CNotificationChannel channel, Guid? tenantId = null);

    /// <summary>
    /// Creates a new channel configuration, optionally scoped to a specific tenant.
    /// </summary>
    Task<ChannelConfigurationDto> CreateAsync(CreateUpdateChannelConfigurationDto input, Guid? tenantId = null);

    /// <summary>
    /// Updates an existing channel configuration by its unique identifier.
    /// </summary>
    Task<ChannelConfigurationDto> UpdateAsync(Guid id, CreateUpdateChannelConfigurationDto input);

    /// <summary>
    /// Deletes a channel configuration by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Enables or disables a channel configuration by its unique identifier.
    /// </summary>
    Task SetEnabledAsync(Guid id, bool isEnabled);
}
