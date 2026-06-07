using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using LHA.Shared.Contracts.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.PermissionManagement.PermissionManagementPermissions;

namespace LHA.Identity.HttpApi;

/// <summary>
/// Maps permission management endpoints under <c>/api/identity/permissions</c>.
/// </summary>
public static class PermissionEndpoints
{
    /// <summary>
    /// Maps permission management endpoints under <c>/api/identity/permissions</c>.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Identity", "/api/v{version:apiVersion}/identity/permissions")
            .WithTags("Identity - Permissions")
            .RequireAuthorization();

        // ── Get permissions for a provider ───────────────────────────
        group.MapGet("/", async (
            [AsParameters] GetPermissionListInput input,
            IPermissionAppService service) =>
        {
            var result = await service.GetAsync(input);
            return Results.Ok(ApiResponse<List<PermissionGrantDto>>.Ok(result));
        })
        .RequireAuthorization(P.Grants.Read)
        .WithName("GetPermissions")
        .WithSummary("Gets permission grants for a provider (role or user).")
        .Produces<ApiResponse<List<PermissionGrantDto>>>();

        // ── Update permissions ───────────────────────────────────────
        group.MapPut("/", async (
            UpdatePermissionsInput input,
            IPermissionAppService service) =>
        {
            await service.UpdateAsync(input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Grants.Manage)
        .WithName("UpdatePermissions")
        .WithSummary("Batch grants/revokes permissions for a provider.")
        .Produces(204);

        return endpoints;
    }
}
