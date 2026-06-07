using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;

using LHA.Ddd.Application;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.HttpApi;

/// <summary>
/// Notification message endpoints.
/// </summary>
public static class NotificationMessageEndpoints
{
    /// <summary>
    /// Maps the notification message endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapNotificationMessageEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        group.MapPost("/send", async (
            SendNotificationDto request,
            INotificationService service) =>
        {
            var result = await service.SendAsync(request);
            return Results.Ok(ApiResponse<NotificationDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Send)
        .WithName("SendNotification")
        .WithSummary("Sends a single notification.");

        group.MapPost("/schedule", async (
            ScheduleNotificationDto request,
            INotificationService service) =>
        {
            var result = await service.ScheduleAsync(request);
            return Results.Ok(ApiResponse<NotificationDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Send)
        .WithName("ScheduleNotification")
        .WithSummary("Schedules a notification for future delivery.");

        group.MapGet("/", async (
            Guid recipientId,
            int page = 1,
            int pageSize = 20,
            INotificationService service = default!) =>
        {
            var result = await service.GetByRecipientAsync(recipientId, page, pageSize);
            return Results.Ok(ApiResponse<NotificationPagedResultDto<NotificationDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Read)
        .WithName("GetNotifications")
        .WithSummary("Gets notifications for a specific recipient.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            INotificationService service) =>
        {
            var result = await service.GetByIdAsync(id, tenantId);
            return result != null 
                ? Results.Ok(ApiResponse<NotificationDto>.Ok(result)) 
                : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Read)
        .WithName("GetNotification")
        .WithSummary("Gets a specific notification by ID.");

        group.MapPost("/{id:guid}/read", async (
            Guid id,
            Guid tenantId,
            INotificationService service) =>
        {
            var result = await service.MarkAsReadAsync(id, tenantId);
            return result != null 
                ? Results.Ok(ApiResponse<NotificationDto>.Ok(result)) 
                : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Read)
        .WithName("MarkAsRead")
        .WithSummary("Marks a notification as read.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            INotificationService service) =>
        {
            var result = await service.DeleteAsync(id, tenantId);
            return result != null 
                ? Results.Ok(ApiResponse<NotificationDto>.Ok(result)) 
                : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Delete)
        .WithName("DeleteNotification")
        .WithSummary("Deletes a notification.");

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            Guid tenantId,
            INotificationService service) =>
        {
            var success = await service.CancelAsync(id, tenantId);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Delete)
        .WithName("CancelNotification")
        .WithSummary("Cancels a pending or scheduled notification.");

        group.MapGet("/unread-count", async (
            Guid recipientId,
            INotificationService service) =>
        {
            var count = await service.GetUnreadCountAsync(recipientId);
            return Results.Ok(ApiResponse<int>.Ok(count));
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Read)
        .WithName("GetUnreadCount")
        .WithSummary("Gets the unread notification count for a recipient.");

        group.MapGet("/search", async (
            Guid recipientId,
            string? query,
            CNotificationType? type,
            CDeliveryStatus? status,
            DateTimeOffset? from,
            DateTimeOffset? to,
            int page = 1,
            int pageSize = 20,
            INotificationService service = default!) =>
        {
            var result = await service.SearchAsync(recipientId, query, type, status, from, to, page, pageSize);
            return Results.Ok(ApiResponse<NotificationPagedResultDto<NotificationSummaryDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Notifications.Read)
        .WithName("SearchNotifications")
        .WithSummary("Searches notifications with filters.");

        return endpoints;
    }
}
