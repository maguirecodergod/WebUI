using LHA.Shared.Contracts.AuditLog;
using LHA.Ddd.Application;
using LHA.Auditing;
using LHA.Ddd.Domain;

namespace LHA.Notification.Application.AuditLogs;

[DisableAuditing]
public class AuditLogAppService : ApplicationService, IAuditLogAppService
{
    private readonly LHA.AuditLog.Application.Contracts.IAuditLogAppService _moduleService;

    public AuditLogAppService(LHA.AuditLog.Application.Contracts.IAuditLogAppService moduleService)
    {
        _moduleService = moduleService;
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account)
    {
        // Notification service only handles its own logs
        return await _moduleService.GetListAsync(input);
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetHostListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account)
    {
        return await _moduleService.GetListAsync(input);
    }

    public virtual async Task<AuditLogDto> GetAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        return await _moduleService.GetAsync(id);
    }

    public virtual Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input, CServiceType service = CServiceType.Account)
    {
        // The base module service doesn't expose a direct GetActionsAsync yet.
        // We can return empty or implement it if needed by querying the repository.
        return Task.FromResult(new PagedResultDto<AuditLogActionDto>(0, new List<AuditLogActionDto>(), input.PageNumber, input.PageSize));
    }

    public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input, CServiceType service = CServiceType.Account)
    {
        return await _moduleService.GetEntityChangesAsync(input);
    }

    public virtual Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input, CServiceType service = CServiceType.Account)
    {
        return Task.FromResult(new PagedResultDto<EntityPropertyChangeDto>(0, new List<EntityPropertyChangeDto>(), input.PageNumber, input.PageSize));
    }

    public virtual Task DeleteAsync(Guid id, CServiceType service = CServiceType.Account)
    {
        throw new BusinessException("NOT_SUPPORTED", "Deletion of audit logs is not supported in the Notification service.");
    }

    public virtual Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime, CServiceType service = CServiceType.Account)
    {
        throw new BusinessException("NOT_SUPPORTED", "Batch deletion of audit logs is not supported in the Notification service.");
    }
}
