using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;

using LHA.Ddd.Application;

namespace LHA.Notification.HttpApi;

public static class DeviceEndpoints
{
    public static IEndpointRouteBuilder MapDeviceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/devices")
            .WithTags("Devices")
            .RequireAuthorization();

        group.MapPost("/register", async (
            RegisterDeviceDto request,
            Guid tenantId,
            Guid userId,
            IDeviceService service) =>
        {
            var result = await service.RegisterAsync(request, tenantId, userId);
            return Results.Ok(ApiResponse<DeviceDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Devices.Manage)
        .WithName("RegisterDevice")
        .WithSummary("Registers a new device or updates an existing one.");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateDeviceDto request,
            Guid tenantId,
            IDeviceService service) =>
        {
            var result = await service.UpdateAsync(id, request, tenantId);
            return Results.Ok(ApiResponse<DeviceDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Devices.Manage)
        .WithName("UpdateDevice")
        .WithSummary("Updates device information.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            IDeviceService service) =>
        {
            var result = await service.DeleteAsync(id, tenantId);
            return result != null ? Results.Ok(ApiResponse<DeviceDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Devices.Manage)
        .WithName("DeleteDevice")
        .WithSummary("Deletes/Deactivates a device.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            IDeviceService service) =>
        {
            var result = await service.GetByIdAsync(id, tenantId);
            return result != null ? Results.Ok(ApiResponse<DeviceDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Devices.Read)
        .WithName("GetDevice")
        .WithSummary("Gets a specific device by ID.");

        group.MapGet("/user/{userId:guid}", async (
            Guid userId,
            Guid tenantId,
            int page = 1,
            int pageSize = 20,
            IDeviceService service = default!) =>
        {
            var result = await service.GetByUserIdAsync(userId, tenantId, page, pageSize);
            return Results.Ok(ApiResponse<DeviceListDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Devices.Read)
        .WithName("GetDevicesByUser")
        .WithSummary("Gets all devices for a specific user.");

        return endpoints;
    }
}
