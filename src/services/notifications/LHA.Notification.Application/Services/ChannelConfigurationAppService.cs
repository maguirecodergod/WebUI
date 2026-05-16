using LHA.Ddd.Application;
using LHA.Shared.Contracts.Notification;
using LHA.Shared.Domain.Enums.Notification;
using LHA.Shared.Domain.Notification;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;

namespace LHA.Notification.Application.Services;

public sealed class ChannelConfigurationAppService : ApplicationService, IChannelConfigurationAppService
{
    private readonly IChannelConfigurationRepository _repository;

    public ChannelConfigurationAppService(IChannelConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChannelConfigurationDto?> GetByChannelAsync(CNotificationChannel channel, Guid? tenantId = null)
    {
        var entity = await _repository.GetAsync(tenantId, channel);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<ChannelConfigurationDto?> GetAsync(Guid id)
    {
        var entity = await _repository.FindAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<PagedResultDto<ChannelConfigurationDto>> GetPagedListAsync(GetChannelConfigurationsInput input)
    {
        // Simple manual paging since IReadOnlyRepository might not support tenantId filter natively in GetListAsync
        var allEntities = await _repository.GetListAsync(input.TenantId, input.IsEnabled);
        
        // Apply manual filters
        var query = allEntities.AsQueryable();
        
        if (input.Channel.HasValue)
        {
            query = query.Where(x => x.Channel == input.Channel.Value);
        }
        
        if (input.ProviderType.HasValue)
        {
            query = query.Where(x => x.ProviderType == input.ProviderType.Value);
        }

        var count = query.Count();
        var pagedEntities = query
            .Skip((input.PageNumber - 1) * input.PageSize)
            .Take(input.PageSize)
            .ToList();
        
        return new PagedResultDto<ChannelConfigurationDto>(
            count,
            pagedEntities.Select(MapToDto).ToList()
        );
    }

    public async Task<List<ChannelConfigurationDto>> GetListAsync(Guid? tenantId = null)
    {
        var entities = await _repository.GetListAsync(tenantId);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<ChannelConfigurationDto> CreateAsync(CreateUpdateChannelConfigurationDto input, Guid? tenantId = null)
    {
        if (await _repository.ExistsAsync(tenantId, input.Channel))
        {
            throw new InvalidOperationException($"Configuration for channel {input.Channel} already exists for tenant {tenantId}");
        }

        var entity = new ChannelConfigurationEntity(
            tenantId,
            input.Channel,
            input.ProviderType,
            input.Settings);

        if (!input.IsEnabled)
        {
            entity.SetEnabled(false);
        }

        await _repository.InsertAsync(entity);
        return MapToDto(entity);
    }

    public async Task<ChannelConfigurationDto> UpdateAsync(Guid id, CreateUpdateChannelConfigurationDto input)
    {
        var entity = await _repository.GetAsync(id);
        if (entity == null)
        {
            throw new InvalidOperationException("Configuration not found");
        }

        entity.Update(
            input.ProviderType,
            input.Settings);

        entity.SetEnabled(input.IsEnabled);

        await _repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task SetEnabledAsync(Guid id, bool isEnabled)
    {
        var entity = await _repository.GetAsync(id);
        if (entity != null)
        {
            entity.SetEnabled(isEnabled);
            await _repository.UpdateAsync(entity);
        }
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
}
