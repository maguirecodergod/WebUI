using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;

using LHA.Ddd.Application;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.HttpApi;

public static class BatchEndpoints
{
    public static IEndpointRouteBuilder MapBatchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/batches")
            .WithTags("Batches")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateBatchDto request,
            Guid tenantId,
            IBatchService service) =>
        {
            var result = await service.CreateAsync(request, tenantId);
            return Results.Ok(ApiResponse<BatchDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Batches.Create)
        .WithName("CreateBatch")
        .WithSummary("Creates a new notification batch.");

        group.MapPost("/{id:guid}/start", async (
            Guid id,
            Guid tenantId,
            IBatchService service) =>
        {
            var result = await service.StartAsync(id, tenantId);
            return Results.Ok(ApiResponse<BatchDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Batches.Manage)
        .WithName("StartBatch")
        .WithSummary("Starts processing a batch.");

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            Guid tenantId,
            IBatchService service) =>
        {
            var result = await service.CancelAsync(id, tenantId);
            return Results.Ok(ApiResponse<BatchDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Batches.Manage)
        .WithName("CancelBatch")
        .WithSummary("Cancels a pending or processing batch.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            IBatchService service) =>
        {
            var result = await service.GetByIdAsync(id, tenantId);
            return result != null ? Results.Ok(ApiResponse<BatchDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Batches.Read)
        .WithName("GetBatch")
        .WithSummary("Gets batch details by ID.");

        group.MapGet("/{id:guid}/progress", async (
            Guid id,
            Guid tenantId,
            IBatchService service) =>
        {
            var result = await service.GetProgressAsync(id, tenantId);
            return Results.Ok(ApiResponse<BatchProgressDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Batches.Read)
        .WithName("GetBatchProgress")
        .WithSummary("Gets the processing progress of a batch.");

        return endpoints;
    }
}
