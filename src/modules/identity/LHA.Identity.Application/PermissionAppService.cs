using LHA.Ddd.Application;
using LHA.EventBus;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.Shared.Contracts.Security;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for permission grant management.
/// </summary>
public sealed class PermissionAppService : ApplicationService, IPermissionAppService
{
    private readonly IPermissionGrantRepository _permissionGrantRepository;
    private readonly IUnitOfWorkManager _uowManager;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly ISecurityVersionManager _securityVersionManager;
    private readonly IEventBus _eventBus;

    public PermissionAppService(
        IPermissionGrantRepository permissionGrantRepository,
        IUnitOfWorkManager uowManager,
        IIdentityRoleRepository roleRepository,
        ISecurityVersionManager securityVersionManager,
        IEventBus eventBus)
    {
        _permissionGrantRepository = permissionGrantRepository;
        _uowManager = uowManager;
        _roleRepository = roleRepository;
        _securityVersionManager = securityVersionManager;
        _eventBus = eventBus;
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
        await PublishSecurityStateChangedAsync(input.ProviderName, input.ProviderKey, ct);
    }

    private async Task PublishSecurityStateChangedAsync(string providerName, string providerKey, CancellationToken ct)
    {
        if (string.Equals(providerName, "U", StringComparison.OrdinalIgnoreCase))
        {
            var version = await _securityVersionManager.BumpUserAsync(providerKey, ct);
            await _eventBus.PublishAsync(
                new SecurityStateChangedEto(SecurityStateTargetType.User, providerKey, version, "user_permissions_changed"),
                ct);
            return;
        }

        if (string.Equals(providerName, "R", StringComparison.OrdinalIgnoreCase))
        {
            var targetId = providerKey;
            if (Guid.TryParse(providerKey, out var roleId))
            {
                var role = await _roleRepository.FindAsync(roleId, ct);
                if (role is not null)
                {
                    targetId = role.Name;
                }
            }

            var version = await _securityVersionManager.BumpRoleAsync(targetId, ct);
            await _eventBus.PublishAsync(
                new SecurityStateChangedEto(SecurityStateTargetType.Role, targetId, version, "role_permissions_changed"),
                ct);
        }
    }
}
