using LHA.Ddd.Application;
using LHA.PermissionManagement.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.PermissionManagement.PermissionManagementPermissions;

namespace LHA.PermissionManagement.HttpApi;

public static class PermissionEndpoints
{
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
        .WithName("GetPermissionDefinitions");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionDefinitionAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionDefinitionDto>.Ok(dto));
        })
        .RequireAuthorization(P.Definitions.Read)
        .WithName("GetPermissionDefinition");

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
        .WithName("FindPermissionDefinitionByName");

        group.MapPost("/register", async (
            List<CreatePermissionDefinitionInput> inputs,
            IPermissionDefinitionAppService service) =>
        {
            var dtos = await service.RegisterAsync(inputs);
            return Results.Ok(ApiResponse<List<PermissionDefinitionDto>>.Ok(dtos));
        })
        .RequireAuthorization(P.Definitions.Manage)
        .WithName("RegisterPermissionDefinitions");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionDefinitionAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Definitions.Manage)
        .WithName("DeletePermissionDefinition");
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
        .WithName("GetPermissionGroups");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionGroupDto>.Ok(dto));
        })
        .RequireAuthorization(P.Groups.Read)
        .WithName("GetPermissionGroup");

        group.MapPost("/", async (
            CreatePermissionGroupInput input,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/permission-groups/{dto.Id}",
                ApiResponse<PermissionGroupDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("CreatePermissionGroup");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePermissionGroupInput input,
            IPermissionGroupAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<PermissionGroupDto>.Ok(dto));
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("UpdatePermissionGroup");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionGroupAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Groups.Manage)
        .WithName("DeletePermissionGroup");
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
        .WithName("GetPermissionTemplates");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<PermissionTemplateDto>.Ok(dto));
        })
        .RequireAuthorization(P.Templates.Read)
        .WithName("GetPermissionTemplate");

        group.MapPost("/", async (
            CreatePermissionTemplateInput input,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/permission-templates/{dto.Id}",
                ApiResponse<PermissionTemplateDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("CreatePermissionTemplate");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdatePermissionTemplateInput input,
            IPermissionTemplateAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<PermissionTemplateDto>.Ok(dto));
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("UpdatePermissionTemplate");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IPermissionTemplateAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Templates.Manage)
        .WithName("DeletePermissionTemplate");
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
        .WithName("GetPermissionGrants");

        group.MapPost("/grant", async (
            GrantPermissionInput input,
            IPermissionGrantAppService service) =>
        {
            await service.GrantAsync(input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Grants.Manage)
        .WithName("GrantPermission");

        group.MapPost("/revoke", async (
            RevokePermissionInput input,
            IPermissionGrantAppService service) =>
        {
            await service.RevokeAsync(input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Grants.Manage)
        .WithName("RevokePermission");

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
        .WithName("CheckPermission");
    }
}
