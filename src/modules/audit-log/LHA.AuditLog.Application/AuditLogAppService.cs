using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.EntityChangeProperties;
using LHA.AuditLog.Application.Mappings;
using LHA.Ddd.Application;
using LHA.Shared.Domain.AuditLogs;
using LHA.Shared.Domain.EntityChanges;
using Microsoft.Extensions.Logging;
using LHA.AuditLog.Application.Contracts;
using LHA.Shared.Domain.AuditLogActions;
using LHA.Shared.Domain.EntityPropertyChanges;

namespace LHA.AuditLog.Application;

/// <summary>
/// Application service for the Audit Log module.
/// <para>
/// Read-only — audit logs are persisted by <c>EfCoreAuditingStore</c> and
/// queried through this service. No create/update/delete operations.
/// </para>
/// </summary>
public class AuditLogAppService : ApplicationService, IAuditLogAppService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IAuditLogActionRepository _auditLogActionRepository;
    private readonly IEntityChangeRepository _entityChangeRepository;
    private readonly IEntityPropertyChangeRepository _entityPropertyChangeRepository;
    private readonly IAuditLogPipelineRepository _auditLogPipelineRepository;
    private readonly ILogger<AuditLogAppService> _logger;

    public AuditLogAppService(
        IAuditLogRepository auditLogRepository,
        IAuditLogActionRepository auditLogActionRepository,
        IEntityChangeRepository entityChangeRepository,
        IEntityPropertyChangeRepository entityPropertyChangeRepository,
        IAuditLogPipelineRepository auditLogPipelineRepository,
        ILogger<AuditLogAppService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _auditLogActionRepository = auditLogActionRepository;
        _entityChangeRepository = entityChangeRepository;
        _entityPropertyChangeRepository = entityPropertyChangeRepository;
        _auditLogPipelineRepository = auditLogPipelineRepository;
        _logger = logger;
    }

    #region Audit logs
    /// <inheritdoc />
    public async virtual Task<AuditLogDto> GetAuditLogDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _auditLogRepository.GetAsync(id, cancellationToken);
        return entity.MapToDto();
    }

    /// <inheritdoc />
    public async virtual Task<PagedResultDto<AuditLogDto>> GetAuditLogWithPaginationAsync(
        AuditLogPagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _auditLogRepository.GetWithPaginationAsync(request: pagedRequest, cancellationToken: cancellationToken);

        return new PagedResultDto<AuditLogDto>(
            pagedResult.TotalCount,
            pagedResult.Items.Select(x => x.MapToDto()).ToList(),
            pagedRequest.PageNumber,
            pagedRequest.PageSize);
    }

    /// <inheritdoc />
    public async virtual Task DeleteAuditLogAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _auditLogRepository.DeleteAsync(id, cancellationToken);
    }


    /// <inheritdoc />
    public async virtual Task<int> DeleteAuditLogOlderThanAsync(DateTimeOffset offset, CancellationToken cancellationToken = default)
    {
        return await _auditLogRepository.DeleteOlderThanAsync(offset, cancellationToken);
    }
    #endregion Audit logs

    #region Audit log actions
    /// <inheritdoc />
    public async virtual Task<AuditLogActionDto> GetAuditLogActionDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _auditLogActionRepository.GetAsync(id, cancellationToken);
        return entity.MapToDto();
    }

    /// <inheritdoc />
    public async virtual Task<PagedResultDto<AuditLogActionDto>> GetAuditLogActionsWithPaginationAsync(
        AuditLogActionPagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _auditLogActionRepository.GetWithPaginationAsync(request: pagedRequest, cancellationToken: cancellationToken);

        return new PagedResultDto<AuditLogActionDto>(
            pagedResult.TotalCount,
            pagedResult.Items.Select(x => x.MapToDto()).ToList(),
            pagedRequest.PageNumber,
            pagedRequest.PageSize);
    }
    #endregion Audit log actions

    #region Entity changes
    /// <inheritdoc />
    public async virtual Task<PagedResultDto<EntityChangeDto>> GetEntityChangesWithPaginationAsync(
        EntityChangePagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _entityChangeRepository.GetWithPaginationAsync(request: pagedRequest, cancellationToken: cancellationToken);

        return new PagedResultDto<EntityChangeDto>(
            pagedResult.TotalCount,
            pagedResult.Items.Select(x => x.MapToDto()).ToList(),
            pagedRequest.PageNumber,
            pagedRequest.PageSize);
    }
    #endregion Entity changes

    #region Entity property changes
    public async virtual Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesWithPaginationAsync(
        EntityPropertyChangePagedRequest pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _entityPropertyChangeRepository.GetWithPaginationAsync(request: pagedRequest, cancellationToken: cancellationToken);

        return new PagedResultDto<EntityPropertyChangeDto>(
            pagedResult.TotalCount,
            pagedResult.Items.Select(x => x.MapToDto()).ToList(),
            pagedRequest.PageNumber,
            pagedRequest.PageSize);
    }
    #endregion Entity property changes
}
