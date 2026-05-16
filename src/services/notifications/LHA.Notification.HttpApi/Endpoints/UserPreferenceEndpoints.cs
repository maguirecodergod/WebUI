using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;

using LHA.Ddd.Application;

namespace LHA.Notification.HttpApi;

public static class UserPreferenceEndpoints
{
    public static IEndpointRouteBuilder MapUserPreferenceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/preferences")
            .WithTags("Preferences")
            .RequireAuthorization();

        group.MapGet("/{userId:guid}", async (
            Guid userId,
            Guid tenantId,
            IUserPreferenceService service) =>
        {
            var result = await service.GetAsync(userId, tenantId);
            return Results.Ok(ApiResponse<UserPreferenceDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Preferences.Read)
        .WithName("GetUserPreference")
        .WithSummary("Gets notification preferences for a user.");

        group.MapPut("/{userId:guid}", async (
            Guid userId,
            UpdatePreferenceDto request,
            Guid tenantId,
            IUserPreferenceService service) =>
        {
            var result = await service.UpdateAsync(userId, request, tenantId);
            return Results.Ok(ApiResponse<UserPreferenceDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Preferences.Update)
        .WithName("UpdateUserPreference")
        .WithSummary("Updates notification preferences for a user.");

        group.MapPost("/{userId:guid}/opt-out", async (
            Guid userId,
            Guid tenantId,
            IUserPreferenceService service) =>
        {
            await service.OptOutAsync(userId, tenantId);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.Preferences.Update)
        .WithName("UserOptOut")
        .WithSummary("Globally opts out a user from all notifications.");

        group.MapPost("/{userId:guid}/opt-in", async (
            Guid userId,
            Guid tenantId,
            IUserPreferenceService service) =>
        {
            await service.OptInAsync(userId, tenantId);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.Preferences.Update)
        .WithName("UserOptIn")
        .WithSummary("Opts in a user to notifications.");

        return endpoints;
    }
}
