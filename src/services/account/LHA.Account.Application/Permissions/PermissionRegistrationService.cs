using LHA.Account.Application.Contracts.Permissions;
using LHA.Identity.Domain;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.Shared;
using LHA.UnitOfWork;

namespace LHA.Account.Application.Permissions;

public sealed class PermissionRegistrationService(
    IPermissionDefinitionRepository defRepo,
    IPermissionGroupRepository groupRepo,
    IIdentityRoleRepository roleRepo,
    LHA.Identity.Domain.IPermissionGrantRepository grantRepo,
    IUnitOfWorkManager uowManager) : IPermissionRegistrationService
{
    public async Task RegisterAsync(RegisterServicePermissionsInput input, CancellationToken ct = default)
    {
        using var uow = uowManager.Begin(isTransactional: true);

        // ── 1. Upsert permission definitions ─────────────────────
        var defEntities = new List<PermissionDefinitionEntity>();
        foreach (var p in input.Permissions)
        {
            var existing = await defRepo.FindByNameAsync(p.Name, ct);
            var side = (MultiTenancySides)p.MultiTenancySide;
            if (existing is null)
            {
                existing = new PermissionDefinitionEntity(
                    Guid.NewGuid(), p.Name, p.DisplayName,
                    input.ServiceName, p.GroupName,
                    multiTenancySide: side);
                await defRepo.InsertAsync(existing, ct);
            }
            else if (existing.MultiTenancySide != side)
            {
                existing.SetMultiTenancySide(side);
                await defRepo.UpdateAsync(existing, ct);
            }
            defEntities.Add(existing);
        }

        // ── 2. Upsert permission groups ──────────────────────────
        foreach (var g in input.Groups)
        {
            var grp = await groupRepo.FindByNameAsync(g.Name, ct);
            if (grp is null)
            {
                grp = new PermissionGroupEntity(
                    Guid.NewGuid(), g.Name, g.DisplayName, input.ServiceName);
                foreach (var def in defEntities.Where(d => d.GroupName == g.Name))
                    grp.AddPermission(def.Id);
                await groupRepo.InsertAsync(grp, ct);
            }
            else
            {
                // Sync new definitions into existing group
                foreach (var def in defEntities.Where(d => d.GroupName == g.Name))
                    grp.AddPermission(def.Id);
                await groupRepo.UpdateAsync(grp, ct);
            }
        }

        // ── 3. Optionally grant all to admin role ────────────────
        if (input.GrantAllToAdminRole)
        {
            var adminRole = await roleRepo.FindByNormalizedNameAsync("ADMIN", ct);
            if (adminRole is not null)
            {
                var adminRoleKey = adminRole.Id.ToString();
                foreach (var def in defEntities)
                {
                    var grant = await grantRepo.FindAsync(def.Name, "R", adminRoleKey, ct);
                    if (grant is null)
                    {
                        await grantRepo.InsertAsync(
                            new IdentityPermissionGrant(
                                Guid.CreateVersion7(), def.Name, "R", adminRoleKey),
                            ct);
                    }
                }
            }
        }

        await uow.CompleteAsync(ct);
    }
}
