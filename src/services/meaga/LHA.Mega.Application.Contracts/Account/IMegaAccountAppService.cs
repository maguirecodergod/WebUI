using LHA.Ddd.Application;

namespace LHA.Mega.Application.Contracts.Account;

public interface IMegaAccountAppService
{
    Task<MegaAccountDto> GetAsync(Guid id, CancellationToken ct = default);

    Task<PagedResultDto<MegaAccountDto>> GetListAsync(
        string? filter,
        bool? isActive,
        string? sorting,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken ct = default);

    Task<MegaAccountDto> CreateAsync(CreateMegaAccountInput input, CancellationToken ct = default);

    Task<MegaAccountDto> UpdateAsync(Guid id, UpdateMegaAccountInput input, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
