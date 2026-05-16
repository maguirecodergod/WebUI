using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface IBatchService
{
    Task<BatchDto> CreateAsync(CreateBatchDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<BatchDto> StartAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<BatchDto> CancelAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<BatchDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<BatchProgressDto> GetProgressAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<BatchDto>> ListAsync(Guid tenantId, CBatchStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<BatchDto>> GetProcessingAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<BatchDto>> GetScheduledAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
