using LHA.Ddd.Domain;

namespace LHA.Mega.Domain.Account;

public interface IMegaAccountRepository : IRepository<MegaAccountEntity, Guid>
{
    Task<MegaAccountEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<List<MegaAccountEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? filter = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filter,
        bool? isActive,
        CancellationToken cancellationToken = default);
}
