using System.Security.Claims;
using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LHA.Identity.HttpApi;

/// <summary>
/// Maps authentication endpoints under <c>/api/identity/auth</c>.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity/auth")
            .WithTags("Identity - Auth");

        // ── Login ────────────────────────────────────────────────────
        group.MapPost("/login", async (
            LoginInput input,
            IAuthAppService service) =>
        {
            var result = await service.LoginAsync(input);
            return Results.Ok(ApiResponse<AuthResultDto>.Ok(result));
        })
        .AllowAnonymous()
        .WithName("Login")
        .WithSummary("Authenticates a user and returns JWT tokens.");

        // ── Register ─────────────────────────────────────────────────
        group.MapPost("/register", async (
            CreateIdentityUserInput input,
            IAuthAppService service) =>
        {
            var dto = await service.RegisterAsync(input);
            return Results.Created($"/api/identity/users/{dto.Id}",
                ApiResponse<IdentityUserDto>.Ok(dto, 201));
        })
        .AllowAnonymous()
        .WithName("Register")
        .WithSummary("Registers a new user account.");

        // ── Refresh Token ────────────────────────────────────────────
        group.MapPost("/refresh", async (
            RefreshTokenInput input,
            IAuthAppService service) =>
        {
            var result = await service.RefreshTokenAsync(input);
            return Results.Ok(ApiResponse<AuthResultDto>.Ok(result));
        })
        .AllowAnonymous()
        .WithName("RefreshToken")
        .WithSummary("Refreshes an access token using a refresh token.");

        // ── Current User ─────────────────────────────────────────────
        group.MapGet("/me", async (
            HttpContext httpContext,
            IAuthAppService service) =>
        {
            var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? httpContext.User.FindFirstValue("sub");

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var dto = await service.GetCurrentUserAsync(userId);
            return Results.Ok(ApiResponse<CurrentUserDto>.Ok(dto));
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithSummary("Gets the currently authenticated user's profile.");

        return endpoints;
    }
}
