using LHA.AuditLog.Application.Contracts;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.AuditLog.AuditLogPermissions;

namespace LHA.AuditLog.HttpApi;

/// <summary>
/// Maps all Audit Log Minimal API endpoints under <c>/api/audit-logs</c>.
/// All responses are wrapped in <see cref="ApiResponse{T}"/>.
/// <para>
/// This module is read-only — no create/update/delete endpoints.
/// </para>
/// </summary>
public static class AuditLogEndpoints
{
    /// <summary>
    /// Maps audit log query endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapAuditLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("AuditLog", "/api/v{version:apiVersion}/audit-logs")
            .WithTags("AuditLogs")
            .RequireAuthorization();

        // ── List (paged + filtered) ──────────────────────────────────
        group.MapGet("/", async (
            [AsParameters] GetAuditLogsInput input,
            LHA.AuditLog.Application.Contracts.IAuditLogAppService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetListAsync(input, cancellationToken);
            return Results.Ok(ApiResponse<PagedResultDto<AuditLogDto>>.Ok(result));
        })
        .RequireAuthorization(P.AuditLogs.Read)
        .WithName("GetAuditLogs")
        .WithSummary("Returns a filtered, paged list of audit logs.");

        // ── Get by ID ────────────────────────────────────────────────
        group.MapGet("/{id:guid}", async (
            Guid id,
            LHA.AuditLog.Application.Contracts.IAuditLogAppService service,
            CancellationToken cancellationToken) =>
        {
            var dto = await service.GetAsync(id, cancellationToken);
            return Results.Ok(ApiResponse<AuditLogDto>.Ok(dto));
        })
        .RequireAuthorization(P.AuditLogs.Read)
        .WithName("GetAuditLog")
        .WithSummary("Gets an audit log by its unique identifier, including actions and entity changes.");

        // ── Entity changes (filtered) ────────────────────────────────
        group.MapGet("/entity-changes", async (
            [AsParameters] GetEntityChangesInput input,
            LHA.AuditLog.Application.Contracts.IAuditLogAppService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetEntityChangesAsync(input, cancellationToken);
            return Results.Ok(ApiResponse<PagedResultDto<EntityChangeDto>>.Ok(result));
        })
        .RequireAuthorization(P.AuditLogs.Read)
        .WithName("GetEntityChanges")
        .WithSummary("Returns entity changes filtered by type and/or entity ID. Useful for viewing entity history.");

        return endpoints;
    }
}
