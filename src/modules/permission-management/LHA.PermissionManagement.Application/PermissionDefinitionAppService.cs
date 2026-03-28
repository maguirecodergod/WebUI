using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.PermissionManagement.Application.Contracts;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.UnitOfWork;

namespace LHA.PermissionManagement.Application;

public sealed class PermissionDefinitionAppService
    : ApplicationService, IPermissionDefinitionAppService
{
    private readonly IPermissionDefinitionRepository _repo;
    private readonly IUnitOfWorkManager _uowManager;

    public PermissionDefinitionAppService(
        IPermissionDefinitionRepository repo,
        IUnitOfWorkManager uowManager)
    {
        _repo = repo;
        _uowManager = uowManager;
    }

    public async Task<PermissionDefinitionDto> GetAsync(Guid id)
    {
        var entity = await _repo.GetAsync(id);
        return MapToDto(entity);
    }

    public async Task<PagedResultDto<PermissionDefinitionDto>> GetListAsync(
        GetPermissionDefinitionsInput input)
    {
        var totalCount = await _repo.GetCountAsync(input.Filter, input.ServiceName, input.GroupName);
        var items = await _repo.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter,
            serviceName: input.ServiceName,
            groupName: input.GroupName);

        return new PagedResultDto<PermissionDefinitionDto>(
            totalCount,
            items.ConvertAll(MapToDto),
            input.PageNumber,
            input.PageSize);
    }

    public async Task<PermissionDefinitionDto?> FindByNameAsync(
        string name, CancellationToken ct = default)
    {
        var entity = await _repo.FindByNameAsync(name, ct);
        return entity is not null ? MapToDto(entity) : null;
    }

    public async Task<List<PermissionDefinitionDto>> RegisterAsync(
        List<CreatePermissionDefinitionInput> inputs, CancellationToken ct = default)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var results = new List<PermissionDefinitionDto>(inputs.Count);

        foreach (var input in inputs)
        {
            var existing = await _repo.FindByNameAsync(input.Name, ct);
            if (existing is not null)
            {
                existing.UpdateDisplayInfo(input.DisplayName, input.Description);
                await _repo.UpdateAsync(existing);
                results.Add(MapToDto(existing));
            }
            else
            {
                var entity = new PermissionDefinitionEntity(
                    Guid.NewGuid(),
                    input.Name,
                    input.DisplayName,
                    input.ServiceName,
                    input.GroupName,
                    input.Description);
                await _repo.InsertAsync(entity);
                results.Add(MapToDto(entity));
            }
        }

        await uow.CompleteAsync();
        return results;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _repo.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    private static PermissionDefinitionDto MapToDto(PermissionDefinitionEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        DisplayName = e.DisplayName,
        ServiceName = e.ServiceName,
        GroupName = e.GroupName,
        Description = e.Description
    };
}
