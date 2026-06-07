using LHA.Shared.Contracts.AuditLog;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using LHA.Account.Application.Contracts.Permissions;
using LHA.Auditing;
using LHA.AuditLog.Application.Contracts;
using LHA.Shared.Domain.AuditLogs;
using LHA.Shared.Domain.AuditLogActions;
using LHA.Shared.Domain.EntityChanges;
using LHA.Shared.Domain.EntityPropertyChanges;

namespace LHA.Account.HttpApi;

public static class AccountAuditLogEndpoints
{
    public static IEndpointRouteBuilder MapAccountAuditLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Account", "/api/v{version:apiVersion}/account/audit-logs")
            .WithTags("AccountAuditLogs")
            .RequireAuthorization()
            .WithMetadata(new DisableAuditingAttribute());

        // CHỈ CẦN 1 ENDPOINT DUY NHẤT:
        // Hệ thống tự động nhận diện Host Admin tại DbContext level để ngắt tenant filter.
        group.MapGet("/", async (
            [AsParameters] AuditLogPagedQuery input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetAuditLogWithPaginationAsync(input.ToRequest());
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLogs")
        .WithSummary("Returns paged audit logs. Automatically handles cross-tenant view for Host Admins via DbContext.")
        .Produces<ApiResponse<PagedResultDto<AuditLogDto>>>();

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetAuditLogDetailAsync(id);
            return Results.Ok(ApiResponse<AuditLogDto>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLog")
        .WithSummary("Gets a specific audit log by ID.")
        .Produces<ApiResponse<AuditLogDto>>();

        group.MapGet("/actions", async (
            [AsParameters] AuditLogActionPagedQuery input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetAuditLogActionsWithPaginationAsync(input.ToRequest());
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogActionDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLogActions")
        .WithSummary("Returns paged audit log actions.")
        .Produces<ApiResponse<PagedResultDto<AuditLogActionDto>>>();

        group.MapGet("/entity-changes", async (
            [AsParameters] EntityChangePagedQuery input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetEntityChangesWithPaginationAsync(input.ToRequest());
            return Results.Ok(ApiResponse<PagedResultDto<EntityChangeDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountEntityChanges")
        .WithSummary("Returns paged entity changes.")
        .Produces<ApiResponse<PagedResultDto<EntityChangeDto>>>();

        group.MapGet("/entity-property-changes", async (
            [AsParameters] EntityPropertyChangePagedQuery input,
            [FromServices] IAuditLogAppService service) =>
        {
            var result = await service.GetEntityPropertyChangesWithPaginationAsync(input.ToRequest());
            return Results.Ok(ApiResponse<PagedResultDto<EntityPropertyChangeDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountEntityPropertyChanges")
        .WithSummary("Returns paged entity property changes.")
        .Produces<ApiResponse<PagedResultDto<EntityPropertyChangeDto>>>();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] IAuditLogAppService service) =>
        {
            await service.DeleteAuditLogAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("DeleteAccountAuditLog")
        .WithSummary("Deletes a specific audit log.")
        .Produces(204);

        group.MapDelete("/older-than", async (
            DateTimeOffset cutoffTime,
            [FromServices] IAuditLogAppService service) =>
        {
            var count = await service.DeleteAuditLogOlderThanAsync(cutoffTime);
            return Results.Ok(ApiResponse<int>.Ok(count));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("DeleteOlderAccountAuditLogs")
        .WithSummary("Deletes audit logs older than the specified date.")
        .Produces<ApiResponse<int>>();

        return endpoints;
    }
}
