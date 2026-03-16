using LHA.Ddd.Domain;

namespace LHA.Mega.Domain.Account;

public interface IMegaAccountRepository : IRepository<MegaAccountEntity, Guid>
{
    Task<MegaAccountEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<List<MegaAccountEntity>> GetListAsync(
        string? filter,
        bool? isActive,
        string? sorting,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filter,
        bool? isActive,
        CancellationToken cancellationToken = default);
}
