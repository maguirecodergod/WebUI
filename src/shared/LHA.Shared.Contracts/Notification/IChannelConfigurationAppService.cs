using LHA.Ddd.Application;
using LHA.Shared.Domain.Enums.Notification;

namespace LHA.Shared.Contracts.Notification;

public interface IChannelConfigurationAppService : IApplicationService
{
    Task<PagedResultDto<ChannelConfigurationDto>> GetPagedListAsync(GetChannelConfigurationsInput input);
    Task<ChannelConfigurationDto?> GetAsync(Guid id);
    Task<ChannelConfigurationDto?> GetByChannelAsync(CNotificationChannel channel, Guid? tenantId = null);
    Task<ChannelConfigurationDto> CreateAsync(CreateUpdateChannelConfigurationDto input, Guid? tenantId = null);
    Task<ChannelConfigurationDto> UpdateAsync(Guid id, CreateUpdateChannelConfigurationDto input);
    Task DeleteAsync(Guid id);
    Task SetEnabledAsync(Guid id, bool isEnabled);
}
