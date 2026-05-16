using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.AuditLog;

public interface IAuditLogAppService : IApplicationService
{
    Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account);

    /// <summary>Host-level: returns ALL audit logs cross-tenant.</summary>
    Task<PagedResultDto<AuditLogDto>> GetHostListAsync(GetAuditLogsInput input, CServiceType service = CServiceType.Account);

    Task<AuditLogDto> GetAsync(Guid id, CServiceType service = CServiceType.Account);

    Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input, CServiceType service = CServiceType.Account);

    Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input, CServiceType service = CServiceType.Account);

    Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input, CServiceType service = CServiceType.Account);

    Task DeleteAsync(Guid id, CServiceType service = CServiceType.Account);

    Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime, CServiceType service = CServiceType.Account);
}
