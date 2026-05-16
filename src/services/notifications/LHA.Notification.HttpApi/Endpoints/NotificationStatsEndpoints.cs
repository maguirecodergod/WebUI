using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;

using LHA.Ddd.Application;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.HttpApi;

public static class NotificationStatsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationStatsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/statistics")
            .WithTags("Statistics")
            .RequireAuthorization();

        group.MapGet("/global", async (
            Guid tenantId,
            DateTimeOffset from,
            DateTimeOffset to,
            INotificationStatsService service) =>
        {
            var result = await service.GetGlobalStatsAsync(tenantId, from, to);
            return Results.Ok(ApiResponse<NotificationStatsDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Statistics.Read)
        .WithName("GetGlobalStats")
        .WithSummary("Gets global notification statistics for a period.");

        group.MapGet("/dashboard", async (
            Guid tenantId,
            INotificationStatsService service) =>
        {
            var result = await service.GetDashboardAsync(tenantId);
            return Results.Ok(ApiResponse<TenantDashboardDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Statistics.Read)
        .WithName("GetDashboardStats")
        .WithSummary("Gets dashboard overview statistics.");

        return endpoints;
    }
}
