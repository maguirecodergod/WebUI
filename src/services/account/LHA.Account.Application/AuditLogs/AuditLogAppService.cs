using LHA.Shared.Contracts.AuditLog;
using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
using LHA.Ddd.Application;

namespace LHA.Account.Application.AuditLogs;

public class AuditLogAppService : ApplicationService, IAuditLogAppService
{
    private readonly LHA.Account.Domain.Repositories.IAuditLogRepository _auditLogRepository;
    private readonly IAuditLogActionRepository _auditLogActionRepository;
    private readonly IEntityChangeRepository _entityChangeRepository;
    private readonly IEntityPropertyChangeRepository _entityPropertyChangeRepository;

    public AuditLogAppService(
        LHA.Account.Domain.Repositories.IAuditLogRepository auditLogRepository,
        IAuditLogActionRepository auditLogActionRepository,
        IEntityChangeRepository entityChangeRepository,
        IEntityPropertyChangeRepository entityPropertyChangeRepository)
    {
        _auditLogRepository = auditLogRepository;
        _auditLogActionRepository = auditLogActionRepository;
        _entityChangeRepository = entityChangeRepository;
        _entityPropertyChangeRepository = entityPropertyChangeRepository;
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input)
    {
        var logs = await _auditLogRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            startTime: input.StartTime,
            endTime: input.EndTime,
            httpMethod: input.HttpMethod,
            url: input.Url,
            userId: input.UserId,
            userName: input.UserName,
            minStatusCode: input.MinStatusCode,
            maxStatusCode: input.MaxStatusCode,
            applicationName: input.ApplicationName,
            correlationId: input.CorrelationId,
            minExecutionDuration: input.MinExecutionDuration,
            maxExecutionDuration: input.MaxExecutionDuration,
            hasException: input.HasException
        );

        var totalCount = await _auditLogRepository.GetCountAsync(
            input.StartTime,
            input.EndTime,
            input.HttpMethod,
            input.Url,
            input.UserId,
            input.UserName,
            input.MinStatusCode,
            input.MaxStatusCode,
            input.ApplicationName,
            input.CorrelationId,
            input.MinExecutionDuration,
            input.MaxExecutionDuration,
            input.HasException
        );

        return new PagedResultDto<AuditLogDto>(
            totalCount,
            logs.Select(MapToDto).ToList(),
            input.PageNumber,
            input.PageSize
        );
    }

    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        var log = await _auditLogRepository.GetAsync(id, includeDetails: true);
        return MapToDto(log);
    }

    public virtual async Task<PagedResultDto<AuditLogActionDto>> GetActionsAsync(GetAuditLogActionsInput input)
    {
        var actions = await _auditLogActionRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            auditLogId: input.AuditLogId,
            serviceName: input.ServiceName,
            methodName: input.MethodName,
            minExecutionDuration: input.MinExecutionDuration,
            maxExecutionDuration: input.MaxExecutionDuration
        );

        var totalCount = await _auditLogActionRepository.GetCountAsync(
            input.AuditLogId,
            input.ServiceName,
            input.MethodName,
            input.MinExecutionDuration,
            input.MaxExecutionDuration
        );

        return new PagedResultDto<AuditLogActionDto>(
            totalCount,
            actions.Select(MapToDto).ToList(),
            input.PageNumber,
            input.PageSize
        );
    }

    public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesInput input)
    {
        var changes = await _entityChangeRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            auditLogId: input.AuditLogId,
            entityTypeFullName: input.EntityTypeFullName,
            entityId: input.EntityId,
            changeType: input.ChangeType,
            includeDetails: true
        );

        var totalCount = await _entityChangeRepository.GetCountAsync(
            input.AuditLogId,
            input.EntityTypeFullName,
            input.EntityId,
            input.ChangeType
        );

        return new PagedResultDto<EntityChangeDto>(
            totalCount,
            changes.Select(MapToDto).ToList(),
            input.PageNumber,
            input.PageSize
        );
    }

    public virtual async Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesAsync(GetEntityPropertyChangesInput input)
    {
        var propChanges = await _entityPropertyChangeRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            entityChangeId: input.EntityChangeId,
            propertyName: input.PropertyName
        );

        var totalCount = await _entityPropertyChangeRepository.GetCountAsync(
            input.EntityChangeId,
            input.PropertyName
        );

        return new PagedResultDto<EntityPropertyChangeDto>(
            totalCount,
            propChanges.Select(MapToDto).ToList(),
            input.PageNumber,
            input.PageSize
        );
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _auditLogRepository.DeleteAsync(id);
    }

    public virtual async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoffTime)
    {
        return await _auditLogRepository.DeleteOlderThanAsync(cutoffTime);
    }

    // ─── Mapping Helpers ─────────────────────────────────────────────

    private AuditLogDto MapToDto(AuditLogEntity entity)
    {
        return new AuditLogDto
        {
            Id = entity.Id,
            ApplicationName = entity.ApplicationName,
            ActionName = entity.ActionName,
            UserId = entity.UserId,
            UserName = entity.UserName,
            TenantId = entity.TenantId,
            TenantName = entity.TenantName,
            ExecutionTime = entity.ExecutionTime,
            ExecutionDuration = entity.ExecutionDuration,
            ClientId = entity.ClientId,
            CorrelationId = entity.CorrelationId,
            ClientIpAddress = entity.ClientIpAddress,
            HttpMethod = entity.HttpMethod,
            HttpStatusCode = entity.HttpStatusCode,
            Url = entity.Url,
            BrowserInfo = entity.BrowserInfo,
            Exceptions = entity.Exceptions,
            Comments = entity.Comments,
            ExtraProperties = entity.ExtraProperties,
            Actions = entity.Actions.Select(MapToDto).ToList(),
            EntityChanges = entity.EntityChanges.Select(MapToDto).ToList()
        };
    }

    private AuditLogActionDto MapToDto(AuditLogActionEntity entity)
    {
        return new AuditLogActionDto
        {
            Id = entity.Id,
            AuditLogId = entity.AuditLogId,
            ServiceName = entity.ServiceName,
            MethodName = entity.MethodName,
            Parameters = entity.Parameters,
            ExecutionTime = entity.ExecutionTime,
            ExecutionDuration = entity.ExecutionDuration
        };
    }

    private EntityChangeDto MapToDto(EntityChangeEntity entity)
    {
        return new EntityChangeDto
        {
            Id = entity.Id,
            AuditLogId = entity.AuditLogId,
            ChangeTime = entity.ChangeTime,
            ChangeType = entity.ChangeType,
            EntityTenantId = entity.EntityTenantId,
            EntityId = entity.EntityId,
            EntityTypeFullName = entity.EntityTypeFullName,
            PropertyChanges = entity.PropertyChanges.Select(MapToDto).ToList()
        };
    }

    private EntityPropertyChangeDto MapToDto(EntityPropertyChangeEntity entity)
    {
        return new EntityPropertyChangeDto
        {
            Id = entity.Id,
            EntityChangeId = entity.EntityChangeId,
            PropertyName = entity.PropertyName,
            PropertyTypeFullName = entity.PropertyTypeFullName,
            OriginalValue = entity.OriginalValue,
            NewValue = entity.NewValue
        };
    }
}
