using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using LHA.UnitOfWork;

namespace LHA.Notification.Application.Batches;

public sealed class BatchAppService : ApplicationService, IBatchService
{
    private readonly INotificationBatchRepository _batchRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public BatchAppService(
        INotificationBatchRepository batchRepository,
        IUnitOfWorkManager uowManager)
    {
        _batchRepository = batchRepository;
        _uowManager = uowManager;
    }

    public async Task<BatchDto> CreateAsync(CreateBatchDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = new NotificationBatchEntity(
            name: request.Name,
            templateId: request.TemplateId?.ToString());

        entity.SetRecipients(request.Recipients.Count);

        await _batchRepository.InsertAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<BatchDto> StartAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _batchRepository.GetAsync(id, cancellationToken);
        entity.StartProcessing();
        await _batchRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<BatchDto> CancelAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _batchRepository.GetAsync(id, cancellationToken);
        entity.Fail("Cancelled by user");
        await _batchRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<BatchDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _batchRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<BatchProgressDto> GetProgressAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _batchRepository.GetAsync(id, cancellationToken);

        var progressPct = entity.TotalCount > 0
            ? (double)(entity.SentCount + entity.FailedCount) / entity.TotalCount * 100
            : 0;

        TimeSpan? estimatedRemaining = null;
        if (entity.StartedAt.HasValue && entity.SentCount > 0)
        {
            var elapsed = DateTimeOffset.UtcNow - entity.StartedAt.Value;
            var rate = entity.SentCount / elapsed.TotalSeconds;
            if (rate > 0)
                estimatedRemaining = TimeSpan.FromSeconds(entity.PendingCount / rate);
        }

        return new BatchProgressDto(
            BatchId: entity.Id,
            TenantId: entity.TenantId ?? Guid.Empty,
            Name: entity.Name,
            Status: entity.Status,
            TotalCount: entity.TotalCount,
            SentCount: entity.SentCount,
            DeliveredCount: entity.DeliveredCount,
            FailedCount: entity.FailedCount,
            PendingCount: entity.PendingCount,
            ProgressPercentage: progressPct,
            StartedAt: entity.StartedAt,
            CompletedAt: entity.CompletedAt,
            EstimatedTimeRemaining: estimatedRemaining);
    }

    public async Task<List<BatchDto>> ListAsync(Guid tenantId, CBatchStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IEnumerable<NotificationBatchEntity> entities;

        if (status.HasValue)
            entities = await _batchRepository.GetByStatusAsync(status.Value, cancellationToken);
        else
            entities = await _batchRepository.GetByTenantAsync(cancellationToken);

        return entities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<List<BatchDto>> GetProcessingAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entities = await _batchRepository.GetProcessingAsync(cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<BatchDto>> GetScheduledAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entities = await _batchRepository.GetScheduledAsync(cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static BatchDto MapToDto(NotificationBatchEntity e) => new(
        Id: e.Id,
        TenantId: e.TenantId ?? Guid.Empty,
        Name: e.Name,
        Status: e.Status,
        TotalCount: e.TotalCount,
        SentCount: e.SentCount,
        DeliveredCount: e.DeliveredCount,
        FailedCount: e.FailedCount,
        PendingCount: e.PendingCount,
        TemplateId: Guid.TryParse(e.TemplateId, out var tid) ? tid : null,
        ScheduledAt: e.ScheduledAt,
        StartedAt: e.StartedAt,
        CompletedAt: e.CompletedAt,
        CreatedAt: e.CreationTime,
        UpdatedAt: e.LastModificationTime ?? e.CreationTime);
}
