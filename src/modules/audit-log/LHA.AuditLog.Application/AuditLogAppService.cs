using LHA.AuditLog.Application.Contracts;
using LHA.AuditLog.Domain;
using LHA.Ddd.Application;

namespace LHA.AuditLog.Application;

/// <summary>
/// Application service for the Audit Log module.
/// <para>
/// Read-only — audit logs are persisted by <c>EfCoreAuditingStore</c> and
/// queried through this service. No create/update/delete operations.
/// </para>
/// </summary>
public sealed class AuditLogAppService : ApplicationService, LHA.AuditLog.Application.Contracts.IAuditLogAppService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogAppService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <inheritdoc />
    public async Task<AuditLogDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _auditLogRepository.GetAsync(id, cancellationToken);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(
        GetAuditLogsInput input,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _auditLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            httpMethod: input.HttpMethod,
            url: input.Url,
            minStatusCode: input.MinStatusCode,
            maxStatusCode: input.MaxStatusCode,
            userId: input.UserId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            cancellationToken: cancellationToken);

        var items = await _auditLogRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            startTime: input.StartTime,
            endTime: input.EndTime,
            httpMethod: input.HttpMethod,
            url: input.Url,
            minStatusCode: input.MinStatusCode,
            maxStatusCode: input.MaxStatusCode,
            userId: input.UserId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            cancellationToken: cancellationToken);

        return new PagedResultDto<AuditLogDto>(
            totalCount,
            items.ConvertAll(MapToDto),
            input.PageNumber,
            input.PageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(
        GetEntityChangesInput input,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _auditLogRepository.GetEntityChangeCountAsync(
            entityTypeFullName: input.EntityTypeFullName,
            entityId: input.EntityId,
            cancellationToken: cancellationToken);

        var items = await _auditLogRepository.GetEntityChangesAsync(
            input,
            sorter: input.Sorter,
            entityTypeFullName: input.EntityTypeFullName,
            entityId: input.EntityId,
            cancellationToken: cancellationToken);

        return new PagedResultDto<EntityChangeDto>(
            totalCount,
            items.ConvertAll(MapToEntityChangeDto),
            input.PageNumber,
            input.PageSize);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static AuditLogDto MapToDto(AuditLogEntity entity)
    {
        return new AuditLogDto
        {
            Id = entity.Id,
            ApplicationName = entity.ApplicationName,
            UserId = entity.UserId,
            UserName = entity.UserName,
            TenantId = entity.TenantId,
            TenantName = entity.TenantName,
            ImpersonatorUserId = entity.ImpersonatorUserId,
            ImpersonatorTenantId = entity.ImpersonatorTenantId,
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
            Actions = entity.Actions.Select(a => new AuditLogActionDto
            {
                Id = a.Id,
                ServiceName = a.ServiceName,
                MethodName = a.MethodName,
                Parameters = a.Parameters,
                ExecutionTime = a.ExecutionTime,
                ExecutionDuration = a.ExecutionDuration
            }).ToList(),
            EntityChanges = entity.EntityChanges.Select(MapToEntityChangeDto).ToList()
        };
    }

    private static EntityChangeDto MapToEntityChangeDto(EntityChangeEntity entity)
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
            PropertyChanges = entity.PropertyChanges.Select(p => new EntityPropertyChangeDto
            {
                Id = p.Id,
                PropertyName = p.PropertyName,
                PropertyTypeFullName = p.PropertyTypeFullName,
                OriginalValue = p.OriginalValue,
                NewValue = p.NewValue
            }).ToList()
        };
    }
}
