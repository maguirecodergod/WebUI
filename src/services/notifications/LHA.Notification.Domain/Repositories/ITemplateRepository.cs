using LHA.Ddd.Domain;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.Repositories;

public interface ITemplateRepository : IRepository<TemplateEntity, Guid>
{
    Task<TemplateEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<TemplateEntity>> GetByTypeAsync(CNotificationType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<TemplateEntity>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TemplateEntity>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TemplateEntity> GetByTenantCursorAsync(Guid? tenantId, int batchSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<TemplateEntity>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
