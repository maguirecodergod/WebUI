using LHA.Ddd.Application;
using LHA.PermissionManagement.Application.Contracts;
using LHA.PermissionManagement.Domain.PermissionGrants;
using LHA.UnitOfWork;

namespace LHA.PermissionManagement.Application;

public sealed class PermissionGrantAppService
    : ApplicationService, IPermissionGrantAppService
{
    private readonly IPermissionGrantRepository _grantRepo;
    private readonly IUnitOfWorkManager _uowManager;

    public PermissionGrantAppService(
        IPermissionGrantRepository grantRepo,
        IUnitOfWorkManager uowManager)
    {
        _grantRepo = grantRepo;
        _uowManager = uowManager;
    }

    public async Task<List<PermissionGrantDto>> GetListAsync(
        GetPermissionGrantsInput input, CancellationToken ct = default)
    {
        var grants = await _grantRepo.GetListAsync(
            input.ProviderName, input.ProviderKey, ct);
        return grants.ConvertAll(MapToDto);
    }

    public async Task GrantAsync(
        GrantPermissionInput input, CancellationToken ct = default)
    {
        var existing = await _grantRepo.FindAsync(
            input.PermissionName, input.ProviderName, input.ProviderKey, ct);
        if (existing is not null) return; // Already granted

        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var grant = new PermissionGrantEntity(
            Guid.NewGuid(),
            input.PermissionName,
            input.ProviderName,
            input.ProviderKey);

        await _grantRepo.InsertAsync(grant);
        await uow.CompleteAsync();
    }

    public async Task RevokeAsync(
        RevokePermissionInput input, CancellationToken ct = default)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _grantRepo.DeleteAsync(
            input.PermissionName, input.ProviderName, input.ProviderKey, ct);
        await uow.CompleteAsync();
    }

    public async Task<bool> IsGrantedAsync(
        string permissionName, string providerName, string providerKey,
        CancellationToken ct = default)
    {
        var grant = await _grantRepo.FindAsync(
            permissionName, providerName, providerKey, ct);
        return grant is not null;
    }

    private static PermissionGrantDto MapToDto(PermissionGrantEntity e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Name = e.Name,
        ProviderName = e.ProviderName,
        ProviderKey = e.ProviderKey
    };
}
