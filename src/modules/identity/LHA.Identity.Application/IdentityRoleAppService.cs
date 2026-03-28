using LHA.Core;
using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for <see cref="IdentityRole"/> management.
/// </summary>
public sealed class IdentityRoleAppService : ApplicationService, IIdentityRoleAppService
{
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IdentityRoleManager _roleManager;
    private readonly IUnitOfWorkManager _uowManager;

    public IdentityRoleAppService(
        IIdentityRoleRepository roleRepository,
        IdentityRoleManager roleManager,
        IUnitOfWorkManager uowManager)
    {
        _roleRepository = roleRepository;
        _roleManager = roleManager;
        _uowManager = uowManager;
    }

    // ─── CRUD ────────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<IdentityRoleDto> GetAsync(Guid id)
    {
        var role = await _roleRepository.GetAsync(id);
        return MapToDto(role);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
    {
        var totalCount = await _roleRepository.GetCountAsync(input.Filter, input.Status);
        var roles = await _roleRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter,
            status: input.Status);

        return new PagedResultDto<IdentityRoleDto>(
            totalCount,
            roles.ConvertAll(MapToDto),
            input.PageNumber,
            input.PageSize);
    }

    /// <inheritdoc />
    public async Task<IdentityRoleDto> CreateAsync(CreateIdentityRoleInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var role = await _roleManager.CreateAsync(input.Name);
        role.SetIsDefault(input.IsDefault);
        role.SetIsPublic(input.IsPublic);

        await _roleRepository.InsertAsync(role);
        await uow.CompleteAsync();

        return MapToDto(role);
    }

    /// <inheritdoc />
    public async Task<IdentityRoleDto> UpdateAsync(Guid id, UpdateIdentityRoleInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var role = await _roleRepository.GetAsync(id);

        if (!string.Equals(role.ConcurrencyStamp, input.ConcurrencyStamp, StringComparison.Ordinal))
            throw new DbUpdateConcurrencyException(
                $"Role '{id}' was modified concurrently. " +
                $"Expected '{input.ConcurrencyStamp}', current '{role.ConcurrencyStamp}'.");

        await _roleManager.ChangeNameAsync(role, input.Name);
        role.SetIsDefault(input.IsDefault);
        role.SetIsPublic(input.IsPublic);

        await _roleRepository.UpdateAsync(role);
        await uow.CompleteAsync();

        return MapToDto(role);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var role = await _roleRepository.GetAsync(id);

        if (role.IsStatic)
            throw new InvalidOperationException($"Cannot delete static role '{role.Name}'.");

        await _roleRepository.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    // ─── Extended ────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<List<IdentityRoleDto>> GetAllAsync(CancellationToken ct)
    {
        var roles = await _roleRepository.GetListAsync(
            paging: new PagingParam { PageSize = 1000 },
            sorter: new SorterParam { KeyName = "Name", IsASC = true },
            cancellationToken: ct);

        return roles.ConvertAll(MapToDto);
    }

    /// <inheritdoc />
    public async Task<IdentityRoleDto> ActivateAsync(Guid id, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var role = await _roleRepository.GetAsync(id, ct);
        role.Activate();
        await _roleRepository.UpdateAsync(role);
        await uow.CompleteAsync();

        return MapToDto(role);
    }

    /// <inheritdoc />
    public async Task<IdentityRoleDto> DeactivateAsync(Guid id, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var role = await _roleRepository.GetAsync(id, ct);
        role.Deactivate();
        await _roleRepository.UpdateAsync(role);
        await uow.CompleteAsync();

        return MapToDto(role);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static IdentityRoleDto MapToDto(IdentityRole role) => new()
    {
        Id = role.Id,
        TenantId = role.TenantId,
        Name = role.Name,
        Status = role.Status,
        IsDefault = role.IsDefault,
        IsStatic = role.IsStatic,
        IsPublic = role.IsPublic,
        ConcurrencyStamp = role.ConcurrencyStamp,
        CreationTime = role.CreationTime,
        CreatorId = role.CreatorId,
        LastModificationTime = role.LastModificationTime,
        LastModifierId = role.LastModifierId,
        IsDeleted = role.IsDeleted,
        DeletionTime = role.DeletionTime,
        DeleterId = role.DeleterId,
    };
}
