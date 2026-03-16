using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Identity.Application.Contracts.IdentityPermissions;

namespace LHA.Identity.HttpApi;

/// <summary>
/// Maps Identity Role management endpoints under <c>/api/identity/roles</c>.
/// </summary>
public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity/roles")
            .WithTags("Identity - Roles")
            .RequireAuthorization();

        // ── List ─────────────────────────────────────────────────────
        group.MapGet("/", async (
            [AsParameters] GetIdentityRolesInput input,
            IIdentityRoleAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<IdentityRoleDto>>.Ok(result));
        })
        .RequireAuthorization(P.Roles.Read)
        .WithName("GetIdentityRoles")
        .WithSummary("Returns a filtered, paged list of roles.");

        // ── All (no paging) ──────────────────────────────────────────
        group.MapGet("/all", async (
            IIdentityRoleAppService service) =>
        {
            var list = await service.GetAllAsync();
            return Results.Ok(ApiResponse<List<IdentityRoleDto>>.Ok(list));
        })
        .RequireAuthorization(P.Roles.Read)
        .WithName("GetAllIdentityRoles")
        .WithSummary("Returns all roles (no paging).");

        // ── Get by ID ────────────────────────────────────────────────
        group.MapGet("/{id:guid}", async (
            Guid id,
            IIdentityRoleAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<IdentityRoleDto>.Ok(dto));
        })
        .RequireAuthorization(P.Roles.Read)
        .WithName("GetIdentityRole")
        .WithSummary("Gets a role by ID.");

        // ── Create ───────────────────────────────────────────────────
        group.MapPost("/", async (
            CreateIdentityRoleInput input,
            IIdentityRoleAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/identity/roles/{dto.Id}",
                ApiResponse<IdentityRoleDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Roles.Create)
        .WithName("CreateIdentityRole")
        .WithSummary("Creates a new role.");

        // ── Update ───────────────────────────────────────────────────
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateIdentityRoleInput input,
            IIdentityRoleAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<IdentityRoleDto>.Ok(dto));
        })
        .RequireAuthorization(P.Roles.Update)
        .WithName("UpdateIdentityRole")
        .WithSummary("Updates a role. Requires concurrency stamp.");

        // ── Delete ───────────────────────────────────────────────────
        group.MapDelete("/{id:guid}", async (
            Guid id,
            IIdentityRoleAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Roles.Delete)
        .WithName("DeleteIdentityRole")
        .WithSummary("Deletes a role (non-static only).");

        // ── Activation ───────────────────────────────────────────────
        group.MapPost("/{id:guid}/activate", async (
            Guid id,
            IIdentityRoleAppService service) =>
        {
            var dto = await service.ActivateAsync(id);
            return Results.Ok(ApiResponse<IdentityRoleDto>.Ok(dto));
        })
        .RequireAuthorization(P.Roles.Update)
        .WithName("ActivateRole")
        .WithSummary("Activates a role.");

        group.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            IIdentityRoleAppService service) =>
        {
            var dto = await service.DeactivateAsync(id);
            return Results.Ok(ApiResponse<IdentityRoleDto>.Ok(dto));
        })
        .RequireAuthorization(P.Roles.Update)
        .WithName("DeactivateRole")
        .WithSummary("Deactivates a role.");

        return endpoints;
    }
}
