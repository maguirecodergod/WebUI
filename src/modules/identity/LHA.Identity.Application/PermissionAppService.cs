using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for permission grant management.
/// </summary>
public sealed class PermissionAppService : ApplicationService, IPermissionAppService
{
    private readonly IPermissionGrantRepository _permissionGrantRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public PermissionAppService(
        IPermissionGrantRepository permissionGrantRepository,
        IUnitOfWorkManager uowManager)
    {
        _permissionGrantRepository = permissionGrantRepository;
        _uowManager = uowManager;
    }

    /// <inheritdoc />
    public async Task<List<PermissionGrantDto>> GetAsync(GetPermissionListInput input, CancellationToken ct)
    {
        var grants = await _permissionGrantRepository.GetListAsync(
            input.ProviderName, input.ProviderKey, ct);

        return grants.ConvertAll(g => new PermissionGrantDto
        {
            Name = g.Name,
            IsGranted = g.IsGranted,
        });
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UpdatePermissionsInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        foreach (var perm in input.Permissions)
        {
            var existing = await _permissionGrantRepository.FindAsync(
                perm.Name, input.ProviderName, input.ProviderKey, ct);

            if (existing is null)
            {
                // Create new grant (allow or deny)
                var grant = new IdentityPermissionGrant(
                    Guid.CreateVersion7(), perm.Name, input.ProviderName, input.ProviderKey,
                    isGranted: perm.IsGranted);
                await _permissionGrantRepository.InsertAsync(grant, ct);
            }
            else if (existing.IsGranted != perm.IsGranted)
            {
                // Changed: delete old, insert new (IsGranted is init-only)
                await _permissionGrantRepository.DeleteAsync(
                    perm.Name, input.ProviderName, input.ProviderKey, ct);
                var grant = new IdentityPermissionGrant(
                    Guid.CreateVersion7(), perm.Name, input.ProviderName, input.ProviderKey,
                    isGranted: perm.IsGranted);
                await _permissionGrantRepository.InsertAsync(grant, ct);
            }
        }

        await uow.CompleteAsync();
    }
}
