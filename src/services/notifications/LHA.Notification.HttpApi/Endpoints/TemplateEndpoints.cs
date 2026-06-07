using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LHA.Notification.Application.Contracts;
using LHA.Ddd.Application;

namespace LHA.Notification.HttpApi;

/// <summary>
/// Notification Template endpoints.
/// </summary>
public static class TemplateEndpoints
{
    /// <summary>
    /// Maps endpoints for Notification Template management.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapTemplateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Notification", "/api/v{version:apiVersion}/notification/templates")
            .WithTags("Templates")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateTemplateDto request,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.CreateAsync(request, tenantId);
            return Results.Ok(ApiResponse<TemplateDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Templates.Create)
        .WithName("CreateTemplate")
        .WithSummary("Creates a new notification template.");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTemplateDto request,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.UpdateAsync(id, request, tenantId);
            return Results.Ok(ApiResponse<TemplateDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Templates.Update)
        .WithName("UpdateTemplate")
        .WithSummary("Updates an existing template.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.DeleteAsync(id, tenantId);
            return result != null ? Results.Ok(ApiResponse<TemplateDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Templates.Delete)
        .WithName("DeleteTemplate")
        .WithSummary("Deletes a template.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.GetByIdAsync(id, tenantId);
            return result != null ? Results.Ok(ApiResponse<TemplateDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Templates.Read)
        .WithName("GetTemplate")
        .WithSummary("Gets a template by ID.");

        group.MapGet("/code/{code}", async (
            string code,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.GetByCodeAsync(code, tenantId);
            return result != null ? Results.Ok(ApiResponse<TemplateDto>.Ok(result)) : Results.NotFound();
        })
        .RequireAuthorization(NotificationPermissions.Templates.Read)
        .WithName("GetTemplateByCode")
        .WithSummary("Gets a template by its unique code.");

        group.MapPost("/preview", async (
            TemplatePreviewDto request,
            Guid tenantId,
            ITemplateService service) =>
        {
            var result = await service.PreviewAsync(request, tenantId);
            return Results.Ok(ApiResponse<TemplateRenderResultDto>.Ok(result));
        })
        .RequireAuthorization(NotificationPermissions.Templates.Read)
        .WithName("PreviewTemplate")
        .WithSummary("Previews a template with variables.");

        return endpoints;
    }
}
