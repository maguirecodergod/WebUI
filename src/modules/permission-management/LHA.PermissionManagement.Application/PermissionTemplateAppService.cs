using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.PermissionManagement.Application.Contracts;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using LHA.UnitOfWork;

namespace LHA.PermissionManagement.Application;

public sealed class PermissionTemplateAppService
    : ApplicationService, IPermissionTemplateAppService
{
    private readonly IPermissionTemplateRepository _templateRepo;
    private readonly IPermissionGroupRepository _groupRepo;
    private readonly IPermissionDefinitionRepository _defRepo;
    private readonly IUnitOfWorkManager _uowManager;

    public PermissionTemplateAppService(
        IPermissionTemplateRepository templateRepo,
        IPermissionGroupRepository groupRepo,
        IPermissionDefinitionRepository defRepo,
        IUnitOfWorkManager uowManager)
    {
        _templateRepo = templateRepo;
        _groupRepo = groupRepo;
        _defRepo = defRepo;
        _uowManager = uowManager;
    }

    public async Task<PermissionTemplateDto> GetAsync(Guid id)
    {
        var template = await _templateRepo.GetAsync(id);
        return await MapToDtoAsync(template);
    }

    public async Task<PagedResultDto<PermissionTemplateDto>> GetListAsync(
        GetPermissionTemplatesInput input)
    {
        var totalCount = await _templateRepo.GetCountAsync(input.Filter);
        var templates = await _templateRepo.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter);

        var dtos = new List<PermissionTemplateDto>(templates.Count);
        foreach (var t in templates)
            dtos.Add(await MapToDtoAsync(t));

        return new PagedResultDto<PermissionTemplateDto>(
            totalCount, dtos, input.PageNumber, input.PageSize);
    }

    public async Task<PermissionTemplateDto> CreateAsync(CreatePermissionTemplateInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var template = new PermissionTemplateEntity(
            Guid.NewGuid(), input.Name, input.DisplayName, input.Description);

        foreach (var groupId in input.GroupIds)
            template.AddGroup(groupId);

        await _templateRepo.InsertAsync(template);
        await uow.CompleteAsync();

        return await MapToDtoAsync(template);
    }

    public async Task<PermissionTemplateDto> UpdateAsync(
        Guid id, UpdatePermissionTemplateInput input)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var template = await _templateRepo.GetAsync(id);
        template.SetDisplayName(input.DisplayName);
        template.SetDescription(input.Description);
        template.SyncGroups(input.GroupIds);

        await _templateRepo.UpdateAsync(template);
        await uow.CompleteAsync();

        return await MapToDtoAsync(template);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _templateRepo.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    public async Task<PermissionTemplateDto?> FindByNameAsync(
        string name, CancellationToken ct = default)
    {
        var template = await _templateRepo.FindByNameAsync(name, ct);
        return template is not null ? await MapToDtoAsync(template) : null;
    }

    private async Task<PermissionTemplateDto> MapToDtoAsync(PermissionTemplateEntity template)
    {
        var groupIds = template.Items.Select(i => i.PermissionGroupId).ToList();
        var groups = new List<PermissionGroupDto>(groupIds.Count);

        if (groupIds.Count > 0)
        {
            // Load each group with its permissions
            foreach (var gid in groupIds)
            {
                var group = await _groupRepo.GetAsync(gid);
                var permIds = group.Items.Select(i => i.PermissionDefinitionId).ToList();
                var perms = permIds.Count > 0 ? await _defRepo.GetListByIdsAsync(permIds) : [];

                groups.Add(new PermissionGroupDto
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
                    Permissions = perms.ConvertAll(p => new PermissionDefinitionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        DisplayName = p.DisplayName,
                        ServiceName = p.ServiceName,
                        GroupName = p.GroupName,
                        Description = p.Description
                    })
                });
            }
        }

        return new PermissionTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            DisplayName = template.DisplayName,
            Description = template.Description,
            CreationTime = template.CreationTime,
            CreatorId = template.CreatorId,
            LastModificationTime = template.LastModificationTime,
            LastModifierId = template.LastModifierId,
            IsDeleted = template.IsDeleted,
            DeletionTime = template.DeletionTime,
            DeleterId = template.DeleterId,
            Groups = groups
        };
    }
}
