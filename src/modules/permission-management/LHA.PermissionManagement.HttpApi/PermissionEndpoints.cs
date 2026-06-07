using LHA.Ddd.Application;
using LHA.PermissionManagement.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.PermissionManagement.PermissionManagementPermissions;

namespace LHA.PermissionManagement.HttpApi;

/// <summary>
/// Permission management endpoints.
/// </summary>
public static class PermissionEndpoints
{
    /// <summary>
    /// Maps the permission management endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapPermissionManagementEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        MapPermissionDefinitionEndpoints(endpoints);
        MapPermissionGroupEndpoints(endpoints);
        MapPermissionTemplateEndpoints(endpoints);
        MapPermissionGrantEndpoints(endpoints);
        return endpoints;
    }

    // ── Permission Definitions ───────────────────────────────────

    private static void MapPermissionDefinitionEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("PermissionManagement", "/api/v{version:apiVersion}/permission-management/definitions")
            .WithTags("PermissionDefinitions")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetPermissionDefinitionsInput input,
            IPermissionDefinitionAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<PermissionDefinitionDto>>.Ok(result));
        })
        .RequireAuthorization(P.Definitions.Read)
        .WithName("GetPermissionDefinitions")
        .WithSummary("Get a list of permission definitions")
        .Produces<ApiResponse<PagedResultDto<PermissionDefinitionDto>>>();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionDefinitionAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionDefinitionDto>.Ok(dto));
        })
        .RequireAuthorization(P.Definitions.Read)
        .WithName("GetPermissionDefinition")
        .WithSummary("Get a specific permission definition by ID")
        .Produces<ApiResponse<PermissionDefinitionDto>>();

        group.MapGet("/by-name/{name}", async (
            string name,
            IPermissionDefinitionAppService service) =>
        {
            var dto = await service.FindByNameAsync(name);
            return dto is not null
                ? Results.Ok(ApiResponse<PermissionDefinitionDto>.Ok(dto))
                : Results.NotFound(ApiResponse<PermissionDefinitionDto>.Fail(
                    404, "PERMISSION_NOT_FOUND", $"Permission '{name}' not found."));
        })
        .RequireAuthorization(P.Definitions.Read)
        .WithName("FindPermissionDefinitionByName")
        .WithSummary("Find a permission definition by its name")
        .Produces<ApiResponse<PermissionDefinitionDto>>();

        group.MapPost("/register", async (
            List<CreatePermissionDefinitionInput> inputs,
            IPermissionDefinitionAppService service) =>
        {
            var dtos = await service.RegisterAsync(inputs);
            return Results.Ok(ApiResponse<List<PermissionDefinitionDto>>.Ok(dtos));
        })
        .RequireAuthorization(P.Definitions.Manage)
        .WithName("RegisterPermissionDefinitions")
        .WithSummary("Register multiple permission definitions")
        .Produces<ApiResponse<List<PermissionDefinitionDto>>>();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionDefinitionAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Definitions.Manage)
        .WithName("DeletePermissionDefinition")
        .WithSummary("Delete a specific permission definition by ID")
        .Produces(204);
    }

    // ── Permission Groups ────────────────────────────────────────

    private static void MapPermissionGroupEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("PermissionManagement", "/api/v{version:apiVersion}/permission-management/groups")
            .WithTags("PermissionGroups")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetPermissionGroupsInput input,
            IPermissionGroupAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<PermissionGroupDto>>.Ok(result));
        })
        .RequireAuthorization(P.Groups.Read)
        .WithName("GetPermissionGroups")
        .WithSummary("Get a list of permission groups")
        .Produces<ApiResponse<PagedResultDto<PermissionGroupDto>>>();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionGroupDto>.Ok(dto));
        })
        .RequireAuthorization(P.Groups.Read)
        .WithName("GetPermissionGroup")
        .WithSummary("Get a specific permission group by ID")
        .Produces<ApiResponse<PermissionGroupDto>>();

        group.MapPost("/", async (
            CreatePermissionGroupInput input,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/permission-groups/{dto.Id}",
                ApiResponse<PermissionGroupDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("CreatePermissionGroup")
        .WithSummary("Create a new permission group")
        .Produces<ApiResponse<PermissionGroupDto>>(201);

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePermissionGroupInput input,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<PermissionGroupDto>.Ok(dto));
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("UpdatePermissionGroup")
        .WithSummary("Update an existing permission group by ID")
        .Produces<ApiResponse<PermissionGroupDto>>();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionGroupAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("DeletePermissionGroup")
        .WithSummary("Delete a specific permission group by ID")
        .Produces(204);
    }

    // ── Permission Templates ─────────────────────────────────────

    private static void MapPermissionTemplateEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("PermissionManagement", "/api/v{version:apiVersion}/permission-management/templates")
            .WithTags("PermissionTemplates")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetPermissionTemplatesInput input,
            IPermissionTemplateAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<PermissionTemplateDto>>.Ok(result));
        })
        .RequireAuthorization(P.Templates.Read)
        .WithName("GetPermissionTemplates")
        .WithSummary("Get a list of permission templates")
        .Produces<ApiResponse<PagedResultDto<PermissionTemplateDto>>>();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionTemplateDto>.Ok(dto));
        })
        .RequireAuthorization(P.Templates.Read)
        .WithName("GetPermissionTemplate")
        .WithSummary("Get a specific permission template by ID")
        .Produces<ApiResponse<PermissionTemplateDto>>();

        group.MapPost("/", async (
            CreatePermissionTemplateInput input,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/permission-templates/{dto.Id}",
                ApiResponse<PermissionTemplateDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("CreatePermissionTemplate")
        .WithSummary("Create a new permission template")
        .Produces<ApiResponse<PermissionTemplateDto>>(201);

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePermissionTemplateInput input,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<PermissionTemplateDto>.Ok(dto));
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("UpdatePermissionTemplate")
        .WithSummary("Update an existing permission template by ID")
        .Produces<ApiResponse<PermissionTemplateDto>>();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionTemplateAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("DeletePermissionTemplate")
        .WithSummary("Delete a specific permission template by ID")
        .Produces(204);
    }

    // ── Permission Grants ────────────────────────────────────────

    private static void MapPermissionGrantEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("PermissionManagement", "/api/v{version:apiVersion}/permission-management/grants")
            .WithTags("PermissionGrants")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetPermissionGrantsInput input,
            IPermissionGrantAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<List<PermissionGrantDto>>.Ok(result));
        })
        .RequireAuthorization(P.Grants.Read)
        .WithName("GetPermissionGrants")
        .WithSummary("Get all permission grants")
        .Produces<ApiResponse<List<PermissionGrantDto>>>();

        group.MapPost("/grant", async (
            GrantPermissionInput input,
            IPermissionGrantAppService service) =>
        {
            await service.GrantAsync(input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Grants.Manage)
        .WithName("GrantPermission")
        .WithSummary("Grant a permission to a subject")
        .Produces(204);

        group.MapPost("/revoke", async (
            RevokePermissionInput input,
            IPermissionGrantAppService service) =>
        {
            await service.RevokeAsync(input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Grants.Manage)
        .WithName("RevokePermission")
        .WithSummary("Revoke a permission from a subject")
        .Produces(204);

        group.MapGet("/check", async (
            string permissionName,
            string providerName,
            string providerKey,
            IPermissionGrantAppService service) =>
        {
            var isGranted = await service.IsGrantedAsync(permissionName, providerName, providerKey);
            return Results.Ok(ApiResponse<bool>.Ok(isGranted));
        })
        .RequireAuthorization(P.Grants.Read)
        .WithName("CheckPermission")
        .WithSummary("Check if a permission is granted to a subject")
        .Produces<ApiResponse<bool>>();
    }
}
