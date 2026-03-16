using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Identity.Application.Contracts.IdentityPermissions;

namespace LHA.Identity.HttpApi;

/// <summary>
/// Maps Identity User management endpoints under <c>/api/identity/users</c>.
/// </summary>
public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity/users")
            .WithTags("Identity - Users")
            .RequireAuthorization();

        // ── List ─────────────────────────────────────────────────────
        group.MapGet("/", async (
            [AsParameters] GetIdentityUsersInput input,
            IIdentityUserAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<IdentityUserDto>>.Ok(result));
        })
        .RequireAuthorization(P.Users.Read)
        .WithName("GetIdentityUsers")
        .WithSummary("Returns a filtered, paged list of users.");

        // ── Get by ID ────────────────────────────────────────────────
        group.MapGet("/{id:guid}", async (
            Guid id,
            IIdentityUserAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto));
        })
        .RequireAuthorization(P.Users.Read)
        .WithName("GetIdentityUser")
        .WithSummary("Gets a user by ID.");

        // ── Find by username ─────────────────────────────────────────
        group.MapGet("/by-username/{userName}", async (
            string userName,
            IIdentityUserAppService service) =>
        {
            var dto = await service.FindByUserNameAsync(userName);
            return dto is not null
                ? Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto))
                : Results.NotFound(ApiResponse<IdentityUserDto>.Fail(404, "USER_NOT_FOUND",
                    $"User '{userName}' not found."));
        })
        .RequireAuthorization(P.Users.Read)
        .WithName("FindUserByUserName")
        .WithSummary("Finds a user by user name.");

        // ── Find by email ────────────────────────────────────────────
        group.MapGet("/by-email/{email}", async (
            string email,
            IIdentityUserAppService service) =>
        {
            var dto = await service.FindByEmailAsync(email);
            return dto is not null
                ? Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto))
                : Results.NotFound(ApiResponse<IdentityUserDto>.Fail(404, "USER_NOT_FOUND",
                    $"User with email '{email}' not found."));
        })
        .RequireAuthorization(P.Users.Read)
        .WithName("FindUserByEmail")
        .WithSummary("Finds a user by email.");

        // ── Create ───────────────────────────────────────────────────
        group.MapPost("/", async (
            CreateIdentityUserInput input,
            IIdentityUserAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/identity/users/{dto.Id}",
                ApiResponse<IdentityUserDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Users.Create)
        .WithName("CreateIdentityUser")
        .WithSummary("Creates a new user.");

        // ── Update ───────────────────────────────────────────────────
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateIdentityUserInput input,
            IIdentityUserAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto));
        })
        .RequireAuthorization(P.Users.Update)
        .WithName("UpdateIdentityUser")
        .WithSummary("Updates a user. Requires concurrency stamp.");

        // ── Delete ───────────────────────────────────────────────────
        group.MapDelete("/{id:guid}", async (
            Guid id,
            IIdentityUserAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Users.Delete)
        .WithName("DeleteIdentityUser")
        .WithSummary("Soft-deletes a user.");

        // ── Roles ────────────────────────────────────────────────────
        group.MapGet("/{id:guid}/roles", async (
            Guid id,
            IIdentityUserAppService service) =>
        {
            var roles = await service.GetRolesAsync(id);
            return Results.Ok(ApiResponse<List<IdentityRoleDto>>.Ok(roles));
        })
        .RequireAuthorization(P.Users.Read)
        .WithName("GetUserRoles")
        .WithSummary("Gets roles assigned to a user.");

        group.MapPut("/{id:guid}/roles", async (
            Guid id,
            List<Guid> roleIds,
            IIdentityUserAppService service) =>
        {
            var dto = await service.UpdateRolesAsync(id, roleIds);
            return Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto));
        })
        .RequireAuthorization(P.Users.Update)
        .WithName("UpdateUserRoles")
        .WithSummary("Updates the roles assigned to a user.");

        // ── Password ─────────────────────────────────────────────────
        group.MapPost("/{id:guid}/change-password", async (
            Guid id,
            ChangePasswordInput input,
            IIdentityUserAppService service) =>
        {
            await service.ChangePasswordAsync(id, input);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Users.Update)
        .WithName("ChangeUserPassword")
        .WithSummary("Changes a user's password.");

        // ── Activation ───────────────────────────────────────────────
        group.MapPost("/{id:guid}/activate", async (
            Guid id,
            IIdentityUserAppService service) =>
        {
            var dto = await service.ActivateAsync(id);
            return Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto));
        })
        .RequireAuthorization(P.Users.Update)
        .WithName("ActivateUser")
        .WithSummary("Activates a user.");

        group.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            IIdentityUserAppService service) =>
        {
            var dto = await service.DeactivateAsync(id);
            return Results.Ok(ApiResponse<IdentityUserDto>.Ok(dto));
        })
        .RequireAuthorization(P.Users.Update)
        .WithName("DeactivateUser")
        .WithSummary("Deactivates a user.");

        return endpoints;
    }
}
