using LHA.Shared.Contracts.AuditLog;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

using LHA.Auditing;

namespace LHA.Notification.HttpApi;

public static class NotificationAuditLogEndpoints
{
    public static IEndpointRouteBuilder MapNotificationAuditLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/audit-logs")
            .WithTags("NotificationAuditLogs")
            .RequireAuthorization()
            .WithMetadata(new DisableAuditingAttribute());

        group.MapGet("/", async (
            [AsParameters] GetAuditLogsInput input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("GetNotificationAuditLogs")
        .WithSummary("Returns paged audit logs for the Notification service.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetAsync(id);
            return Results.Ok(ApiResponse<AuditLogDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("GetNotificationAuditLog")
        .WithSummary("Gets a specific audit log by ID.");

        group.MapGet("/actions", async (
            [AsParameters] GetAuditLogActionsInput input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetActionsAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogActionDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("GetNotificationAuditLogActions")
        .WithSummary("Returns paged audit log actions.");

        group.MapGet("/entity-changes", async (
            [AsParameters] GetEntityChangesInput input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetEntityChangesAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<EntityChangeDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("GetNotificationEntityChanges")
        .WithSummary("Returns paged entity changes.");

        group.MapGet("/entity-property-changes", async (
            [AsParameters] GetEntityPropertyChangesInput input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetEntityPropertyChangesAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<EntityPropertyChangeDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("GetNotificationEntityPropertyChanges")
        .WithSummary("Returns paged entity property changes.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] IAuditLogAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("DeleteNotificationAuditLog")
        .WithSummary("Deletes a specific audit log.");

        group.MapDelete("/older-than", async (
            DateTimeOffset cutoffTime,
            [FromServices] IAuditLogAppService service) =>
        {
            var count = await service.DeleteOlderThanAsync(cutoffTime);
            return Results.Ok(ApiResponse<int>.Ok(count));
        })
        .RequireAuthorization(NotificationPermissions.AuditLogs.Read)
        .WithName("DeleteOlderNotificationAuditLogs")
        .WithSummary("Deletes audit logs older than the specified date.");

        return endpoints;
    }
}
