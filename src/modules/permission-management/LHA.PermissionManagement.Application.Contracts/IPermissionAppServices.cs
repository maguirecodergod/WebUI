using LHA.Ddd.Application;

namespace LHA.PermissionManagement.Application.Contracts;

// ──────────────────────────────────────────────────────────────────
//  PermissionDefinition Service
// ──────────────────────────────────────────────────────────────────

public interface IPermissionDefinitionAppService
{
    Task<PermissionDefinitionDto> GetAsync(Guid id);
    Task<PagedResultDto<PermissionDefinitionDto>> GetListAsync(GetPermissionDefinitionsInput input);
    Task<PermissionDefinitionDto?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionDefinitionDto>> RegisterAsync(
        List<CreatePermissionDefinitionInput> inputs, CancellationToken ct = default);
    Task DeleteAsync(Guid id);
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGroup Service
// ──────────────────────────────────────────────────────────────────

public interface IPermissionGroupAppService
    : ICrudAppService<PermissionGroupDto, Guid, GetPermissionGroupsInput,
        CreatePermissionGroupInput, UpdatePermissionGroupInput>
{
    Task<PermissionGroupDto?> FindByNameAsync(string name, CancellationToken ct = default);
}

// ──────────────────────────────────────────────────────────────────
//  PermissionTemplate Service
// ──────────────────────────────────────────────────────────────────

public interface IPermissionTemplateAppService
    : ICrudAppService<PermissionTemplateDto, Guid, GetPermissionTemplatesInput,
        CreatePermissionTemplateInput, UpdatePermissionTemplateInput>
{
    Task<PermissionTemplateDto?> FindByNameAsync(string name, CancellationToken ct = default);
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGrant Service
// ──────────────────────────────────────────────────────────────────

public interface IPermissionGrantAppService
{
    Task<List<PermissionGrantDto>> GetListAsync(GetPermissionGrantsInput input, CancellationToken ct = default);
    Task GrantAsync(GrantPermissionInput input, CancellationToken ct = default);
    Task RevokeAsync(RevokePermissionInput input, CancellationToken ct = default);
    Task<bool> IsGrantedAsync(string permissionName, string providerName, string providerKey, CancellationToken ct = default);
}
