using LHA.Ddd.Application;
using LHA.PermissionManagement.Application.Contracts;
using LHA.PermissionManagement.Domain;
using LHA.UnitOfWork;

namespace LHA.PermissionManagement.Application;

public sealed class PermissionGroupAppService
    : ApplicationService, IPermissionGroupAppService
{
    private readonly IPermissionGroupRepository _groupRepo;
    private readonly IPermissionDefinitionRepository _defRepo;
    private readonly IUnitOfWorkManager _uowManager;

    public PermissionGroupAppService(
        IPermissionGroupRepository groupRepo,
        IPermissionDefinitionRepository defRepo,
        IUnitOfWorkManager uowManager)
    {
        _groupRepo = groupRepo;
        _defRepo = defRepo;
        _uowManager = uowManager;
    }

    public async Task<PermissionGroupDto> GetAsync(Guid id)
    {
        var group = await _groupRepo.GetAsync(id);
        var permissionIds = group.Items.Select(i => i.PermissionDefinitionId).ToList();
        var permissions = await _defRepo.GetListByIdsAsync(permissionIds);
        return MapToDto(group, permissions);
    }

    public async Task<PagedResultDto<PermissionGroupDto>> GetListAsync(
        GetPermissionGroupsInput input)
    {
        var totalCount = await _groupRepo.GetCountAsync(input.Filter, input.ServiceName);
        var groups = await _groupRepo.GetListAsync(
            filter: input.Filter,
            serviceName: input.ServiceName,
            sorting: input.Sorting,
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount);

        // Batch-load all permission definitions referenced by these groups
        var allPermIds = groups.SelectMany(g => g.Items.Select(i => i.PermissionDefinitionId)).Distinct().ToList();
        var allPerms = allPermIds.Count > 0 ? await _defRepo.GetListByIdsAsync(allPermIds) : [];
        var permLookup = allPerms.ToDictionary(p => p.Id);

        var dtos = groups.ConvertAll(g =>
        {
            var perms = g.Items
                .Where(i => permLookup.ContainsKey(i.PermissionDefinitionId))
                .Select(i => permLookup[i.PermissionDefinitionId])
                .ToList();
            return MapToDto(g, perms);
        });

        return new PagedResultDto<PermissionGroupDto>(
            totalCount, dtos, input.SkipCount, input.MaxResultCount);
    }

    public async Task<PermissionGroupDto> CreateAsync(CreatePermissionGroupInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var group = new PermissionGroup(
            Guid.NewGuid(), input.Name, input.DisplayName, input.ServiceName, input.Description);

        foreach (var permId in input.PermissionDefinitionIds)
            group.AddPermission(permId);

        await _groupRepo.InsertAsync(group);
        await uow.CompleteAsync();

        var permissions = await _defRepo.GetListByIdsAsync(input.PermissionDefinitionIds);
        return MapToDto(group, permissions);
    }

    public async Task<PermissionGroupDto> UpdateAsync(Guid id, UpdatePermissionGroupInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var group = await _groupRepo.GetAsync(id);
        group.SetDisplayName(input.DisplayName);
        group.SetDescription(input.Description);
        group.SyncPermissions(input.PermissionDefinitionIds);

        await _groupRepo.UpdateAsync(group);
        await uow.CompleteAsync();

        var permissions = await _defRepo.GetListByIdsAsync(input.PermissionDefinitionIds);
        return MapToDto(group, permissions);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _groupRepo.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    public async Task<PermissionGroupDto?> FindByNameAsync(
        string name, CancellationToken ct = default)
    {
        var group = await _groupRepo.FindByNameAsync(name, ct);
        if (group is null) return null;

        var permissions = await _defRepo.GetListByIdsAsync(
            group.Items.Select(i => i.PermissionDefinitionId).ToList());
        return MapToDto(group, permissions);
    }

    private static PermissionGroupDto MapToDto(
        PermissionGroup group, List<PermissionDefinition> permissions) => new()
    {
        Id = group.Id,
        Name = group.Name,
        DisplayName = group.DisplayName,
        ServiceName = group.ServiceName,
        Description = group.Description,
        CreationTime = group.CreationTime,
        CreatorId = group.CreatorId,
        LastModificationTime = group.LastModificationTime,
        LastModifierId = group.LastModifierId,
        IsDeleted = group.IsDeleted,
        DeletionTime = group.DeletionTime,
        DeleterId = group.DeleterId,
        Permissions = permissions.ConvertAll(p => new PermissionDefinitionDto
        {
            Id = p.Id,
            Name = p.Name,
            DisplayName = p.DisplayName,
            ServiceName = p.ServiceName,
            GroupName = p.GroupName,
            Description = p.Description
        })
    };
}
