using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionTemplates;

public interface IPermissionTemplateRepository : IRepository<PermissionTemplateEntity, Guid>
{
    Task<PermissionTemplateEntity?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionTemplateEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null,
        CancellationToken ct = default);
    Task<long> GetCountAsync(string? filter = null, CancellationToken ct = default);
}
