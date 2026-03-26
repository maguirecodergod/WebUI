using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.AuditLog;

public interface IAuditLogAppService : IApplicationService
{
    Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input);

    Task<AuditLogDto> GetAsync(Guid id);

    Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input);

    Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input);

    Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input);

    Task DeleteAsync(Guid id);

    Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime);
}
