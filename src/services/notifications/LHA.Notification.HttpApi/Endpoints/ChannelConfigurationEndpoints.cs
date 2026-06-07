using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using LHA.Ddd.Application;
using LHA.MultiTenancy;

namespace LHA.Notification.HttpApi;

/// <summary>
/// Channel Configuration endpoints.
/// </summary>
public static class ChannelConfigurationEndpoints
{
    /// <summary>
    /// Maps endpoints for Channel Configuration management.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapChannelConfigurationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/configurations")
            .WithTags("Configurations")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetChannelConfigurationsInput input,
            [FromServices] IChannelConfigurationAppService service = default!) =>
        {
            var result = await service.GetPagedListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<ChannelConfigurationDto>>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Read)
        .WithName("GetChannelConfigurations")
        .WithSummary("Gets paged list of channel configurations. Super admin can query across tenants.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            var result = await service.GetAsync(id);
            return result != null ? Results.Ok(ApiResponse<ChannelConfigurationDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Read)
        .WithName("GetChannelConfigurationById")
        .WithSummary("Gets detailed channel configuration by ID and optionally Tenant.");

        group.MapGet("/channel/{channel}", async (
            CNotificationChannel channel,
            Guid? tenantId,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            var result = await service.GetByChannelAsync(channel, tenantId);
            return result != null ? Results.Ok(ApiResponse<ChannelConfigurationDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Read)
        .WithName("GetChannelConfigurationByChannel")
        .WithSummary("Gets the configuration for a specific notification channel.");

        group.MapGet("/current/{channel}", async (
            CNotificationChannel channel,
            [FromServices] ICurrentTenant currentTenant,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            var result = await service.GetByChannelAsync(channel, currentTenant.Id);
            return result != null ? Results.Ok(ApiResponse<ChannelConfigurationDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Read)
        .WithName("GetCurrentChannelConfiguration")
        .WithSummary("Gets the channel configuration for the current tenant.");

        group.MapPost("/", async (
            CreateUpdateChannelConfigurationDto request,
            Guid? tenantId,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            var result = await service.CreateAsync(request, tenantId);
            return Results.Ok(ApiResponse<ChannelConfigurationDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Manage)
        .WithName("CreateChannelConfiguration")
        .WithSummary("Creates a new channel configuration.");

        group.MapPut("/{id:guid}", async (
            Guid id,
            CreateUpdateChannelConfigurationDto request,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            var result = await service.UpdateAsync(id, request);
            return Results.Ok(ApiResponse<ChannelConfigurationDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Manage)
        .WithName("UpdateChannelConfiguration")
        .WithSummary("Updates an existing channel configuration.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Manage)
        .WithName("DeleteChannelConfiguration")
        .WithSummary("Deletes a channel configuration.");

        group.MapPost("/{id:guid}/enable", async (
            Guid id,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            await service.SetEnabledAsync(id, true);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Manage)
        .WithName("EnableChannelConfiguration")
        .WithSummary("Enables a channel configuration.");

        group.MapPost("/{id:guid}/disable", async (
            Guid id,
            [FromServices] IChannelConfigurationAppService service) =>
        {
            await service.SetEnabledAsync(id, false);
            return Results.NoContent();
        })
        .RequireAuthorization(NotificationPermissions.Configuration.Manage)
        .WithName("DisableChannelConfiguration")
        .WithSummary("Disables a channel configuration.");

        return endpoints;
    }
}
