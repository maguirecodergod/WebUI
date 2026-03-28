using LHA.Core;
using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for <see cref="IdentityUser"/> management.
/// </summary>
public sealed class IdentityUserAppService : ApplicationService, IIdentityUserAppService
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IUnitOfWorkManager _uowManager;

    public IdentityUserAppService(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IdentityUserManager userManager,
        IUnitOfWorkManager uowManager)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _uowManager = uowManager;
    }

    // ─── CRUD ────────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<IdentityUserDto> GetAsync(Guid id)
    {
        var user = await _userRepository.GetAsync(id);
        return await MapToDtoAsync(user);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
    {
        var totalCount = await _userRepository.GetCountAsync(input.Filter, input.Status, input.RoleId);
        var users = await _userRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter,
            status: input.Status,
            roleId: input.RoleId);

        var dtos = new List<IdentityUserDto>(users.Count);
        foreach (var user in users)
            dtos.Add(await MapToDtoAsync(user));

        return new PagedResultDto<IdentityUserDto>(
            totalCount, dtos, input.PageNumber, input.PageSize);
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> CreateAsync(CreateIdentityUserInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userManager.CreateAsync(
            input.UserName, input.Email, input.Password);

        if (input.PhoneNumber is not null) user.SetPhoneNumber(input.PhoneNumber);
        if (input.Name is not null) user.SetName(input.Name);
        if (input.Surname is not null) user.SetSurname(input.Surname);

        // Assign extra roles (beyond defaults)
        foreach (var roleId in input.RoleIds)
            user.AddRole(roleId);

        await _userRepository.InsertAsync(user);
        await uow.CompleteAsync();

        return await MapToDtoAsync(user);
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> UpdateAsync(Guid id, UpdateIdentityUserInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.GetAsync(id);

        if (!string.Equals(user.ConcurrencyStamp, input.ConcurrencyStamp, StringComparison.Ordinal))
            throw new DbUpdateConcurrencyException(
                $"User '{id}' was modified concurrently. " +
                $"Expected '{input.ConcurrencyStamp}', current '{user.ConcurrencyStamp}'.");

        await _userManager.ChangeUserNameAsync(user, input.UserName);
        await _userManager.ChangeEmailAsync(user, input.Email);

        user.SetPhoneNumber(input.PhoneNumber);
        user.SetName(input.Name);
        user.SetSurname(input.Surname);

        // Reconcile roles
        SyncRoles(user, input.RoleIds);

        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();

        return await MapToDtoAsync(user);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _userRepository.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    // ─── Extended Operations ─────────────────────────────────────────

    /// <inheritdoc />
    public async Task<IdentityUserDto?> FindByUserNameAsync(string userName, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        var normalized = userName.Trim().ToUpperInvariant();
        var user = await _userRepository.FindByNormalizedUserNameAsync(normalized, ct);
        return user is not null ? await MapToDtoAsync(user) : null;
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto?> FindByEmailAsync(string email, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        var normalized = email.Trim().ToUpperInvariant();
        var user = await _userRepository.FindByNormalizedEmailAsync(normalized, ct);
        return user is not null ? await MapToDtoAsync(user) : null;
    }

    /// <inheritdoc />
    public async Task<List<IdentityRoleDto>> GetRolesAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetAsync(userId, ct);
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        var roles = await _roleRepository.GetListByIdsAsync(roleIds, ct);

        return roles.ConvertAll(r => new IdentityRoleDto
        {
            Id = r.Id,
            TenantId = r.TenantId,
            Name = r.Name,
            Status = r.Status,
            IsDefault = r.IsDefault,
            IsStatic = r.IsStatic,
            IsPublic = r.IsPublic,
            ConcurrencyStamp = r.ConcurrencyStamp,
        });
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> UpdateRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.GetAsync(userId, ct);
        SyncRoles(user, roleIds);
        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();

        return await MapToDtoAsync(user);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.GetAsync(userId, ct);

        if (!_userManager.VerifyPassword(user, input.CurrentPassword))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        _userManager.ChangePassword(user, input.NewPassword);
        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> ActivateAsync(Guid id, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.GetAsync(id, ct);
        user.Activate();
        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();

        return await MapToDtoAsync(user);
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> DeactivateAsync(Guid id, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.GetAsync(id, ct);
        user.Deactivate();
        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();

        return await MapToDtoAsync(user);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private async Task<IdentityUserDto> MapToDtoAsync(IdentityUser user)
    {
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
        var roles = roleIds.Count > 0
            ? await _roleRepository.GetListByIdsAsync(roleIds)
            : [];

        return new IdentityUserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            AccessFailedCount = user.AccessFailedCount,
            Status = user.Status,
            Name = user.Name,
            Surname = user.Surname,
            ConcurrencyStamp = user.ConcurrencyStamp,
            CreationTime = user.CreationTime,
            CreatorId = user.CreatorId,
            LastModificationTime = user.LastModificationTime,
            LastModifierId = user.LastModifierId,
            IsDeleted = user.IsDeleted,
            DeletionTime = user.DeletionTime,
            DeleterId = user.DeleterId,
            Roles = roles.ConvertAll(r => new IdentityUserRoleDto
            {
                RoleId = r.Id,
                RoleName = r.Name,
            }),
        };
    }

    private static void SyncRoles(IdentityUser user, List<Guid> desiredRoleIds)
    {
        // Remove roles not in the desired list
        var currentRoleIds = user.Roles.Select(r => r.RoleId).ToList();
        foreach (var removeId in currentRoleIds.Except(desiredRoleIds))
            user.RemoveRole(removeId);

        // Add new roles
        foreach (var addId in desiredRoleIds.Except(currentRoleIds))
            user.AddRole(addId);
    }
}

/// <summary>
/// Pseudo-exception for EF Core concurrency conflicts (avoid pulling EF Core dependency).
/// </summary>
public sealed class DbUpdateConcurrencyException : Exception
{
    public DbUpdateConcurrencyException(string message) : base(message) { }
}
