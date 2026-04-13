using LHA.Shared.Contracts.AuditLog;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Account.Application.Contracts.Permissions;
using LHA.Auditing;

namespace LHA.Account.HttpApi;

public static class AccountAuditLogEndpoints
{
    public static IEndpointRouteBuilder MapAccountAuditLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Account", "/api/v{version:apiVersion}/account/audit-logs")
            .WithTags("AccountAuditLogs")
            .RequireAuthorization()
            .WithMetadata(new DisableAuditingAttribute()); ;

        group.MapGet("/", async (
            [AsParameters] GetAuditLogsInput input,
            IAuditLogAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLogs")
        .WithSummary("Returns paged audit logs managed by Account service.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IAuditLogAppService service) =>
        {
            var result = await service.GetAsync(id);
            return Results.Ok(ApiResponse<AuditLogDto>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLog")
        .WithSummary("Gets a specific audit log by ID.");

        group.MapGet("/actions", async (
            [AsParameters] GetAuditLogActionsInput input,
            IAuditLogAppService service) =>
        {
            var result = await service.GetActionsAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogActionDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountAuditLogActions")
        .WithSummary("Returns paged audit log actions.");

        group.MapGet("/entity-changes", async (
            [AsParameters] GetEntityChangesInput input,
            IAuditLogAppService service) =>
        {
            var result = await service.GetEntityChangesAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<EntityChangeDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountEntityChanges")
        .WithSummary("Returns paged entity changes.");

        group.MapGet("/entity-property-changes", async (
            [AsParameters] GetEntityPropertyChangesInput input,
            IAuditLogAppService service) =>
        {
            var result = await service.GetEntityPropertyChangesAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<EntityPropertyChangeDto>>.Ok(result));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("GetAccountEntityPropertyChanges")
        .WithSummary("Returns paged entity property changes.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IAuditLogAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read) // Should probably have a Delete permission but we'll use Read for now as per base or add a new one
        .WithName("DeleteAccountAuditLog")
        .WithSummary("Deletes a specific audit log.");

        group.MapDelete("/older-than", async (
            DateTimeOffset cutoffTime,
            IAuditLogAppService service) =>
        {
            var count = await service.DeleteOlderThanAsync(cutoffTime);
            return Results.Ok(ApiResponse<int>.Ok(count));
        })
        .RequireAuthorization(AccountPermissions.AuditLogManagement.Read)
        .WithName("DeleteOlderAccountAuditLogs")
        .WithSummary("Deletes audit logs older than the specified date.");

        return endpoints;
    }
}
