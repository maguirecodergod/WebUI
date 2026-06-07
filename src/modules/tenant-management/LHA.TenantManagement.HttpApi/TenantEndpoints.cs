using LHA.Ddd.Application;
using LHA.TenantManagement.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.TenantManagement.TenantManagementPermissions;

namespace LHA.TenantManagement.HttpApi;

/// <summary>
/// Maps all Tenant Management Minimal API endpoints under <c>/api/tenants</c>.
/// All responses are wrapped in <see cref="ApiResponse{T}"/>.
/// </summary>
public static class TenantEndpoints
{
    /// <summary>
    /// Maps tenant CRUD, connection string management, and activation endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("TenantManagement", "/api/v{version:apiVersion}/tenant-management")
            .WithTags("Tenants")
            .RequireAuthorization();

        // ── List (paged + filtered) ──────────────────────────────────
        group.MapGet("/", async (
            [AsParameters] GetTenantsInput input,
            ITenantAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<TenantDto>>.Ok(result));
        })
        .RequireAuthorization(P.Tenants.Read)
        .WithName("GetTenants")
        .WithSummary("Returns a filtered, paged list of tenants.")
        .Produces<ApiResponse<PagedResultDto<TenantDto>>>();

        // ── Get by ID ────────────────────────────────────────────────
        group.MapGet("/{id:guid}", async (
            Guid id,
            ITenantAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Read)
        .WithName("GetTenant")
        .WithSummary("Gets a tenant by its unique identifier.")
        .Produces<ApiResponse<TenantDto>>();

        // ── Find by name ─────────────────────────────────────────────
        group.MapGet("/by-name/{name}", async (
            string name,
            ITenantAppService service) =>
        {
            var dto = await service.FindByNameAsync(name);
            return dto is not null
                ? Results.Ok(ApiResponse<TenantDto>.Ok(dto))
                : Results.NotFound(ApiResponse<TenantDto>.Fail(404, "TENANT_NOT_FOUND", $"Tenant '{name}' not found."));
        })
        .RequireAuthorization(P.Tenants.Read)
        .WithName("FindTenantByName")
        .WithSummary("Finds a tenant by display name (case-insensitive).")
        .Produces<ApiResponse<TenantDto>>();

        // ── Create ───────────────────────────────────────────────────
        group.MapPost("/", async (
            CreateTenantInput input,
            ITenantAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/tenants/{dto.Id}", ApiResponse<TenantDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Tenants.Create)
        .WithName("CreateTenant")
        .WithSummary("Creates a new tenant.")
        .Produces<ApiResponse<TenantDto>>(201);

        // ── Update ───────────────────────────────────────────────────
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTenantInput input,
            ITenantAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("UpdateTenant")
        .WithSummary("Updates a tenant (name). Requires concurrency stamp.")
        .Produces<ApiResponse<TenantDto>>();

        // ── Delete ───────────────────────────────────────────────────
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ITenantAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Tenants.Delete)
        .WithName("DeleteTenant")
        .WithSummary("Soft-deletes a tenant.")
        .Produces(204);

        // ── Connection Strings ───────────────────────────────────────
        group.MapGet("/{id:guid}/connection-strings", async (
            Guid id,
            ITenantAppService service) =>
        {
            var list = await service.GetConnectionStringsAsync(id);
            return Results.Ok(ApiResponse<List<TenantConnectionStringDto>>.Ok(list));
        })
        .RequireAuthorization(P.Tenants.Read)
        .WithName("GetTenantConnectionStrings")
        .WithSummary("Returns all connection strings for a tenant.")
        .Produces<ApiResponse<List<TenantConnectionStringDto>>>();

        group.MapPut("/{id:guid}/connection-strings/{name}", async (
            Guid id,
            string name,
            SetConnectionStringInput input,
            ITenantAppService service) =>
        {
            var dto = await service.SetConnectionStringAsync(id, name, input.Value);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("SetTenantConnectionString")
        .WithSummary("Adds or updates a named connection string on the tenant.")
        .Produces<ApiResponse<TenantDto>>();

        group.MapDelete("/{id:guid}/connection-strings/{name}", async (
            Guid id,
            string name,
            ITenantAppService service) =>
        {
            var dto = await service.RemoveConnectionStringAsync(id, name);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("RemoveTenantConnectionString")
        .WithSummary("Removes a named connection string from the tenant.")
        .Produces<ApiResponse<TenantDto>>();

        // ── Activation ───────────────────────────────────────────────
        group.MapPost("/{id:guid}/activate", async (
            Guid id,
            ITenantAppService service) =>
        {
            var dto = await service.ActivateAsync(id);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("ActivateTenant")
        .WithSummary("Activates a tenant.")
        .Produces<ApiResponse<TenantDto>>();

        group.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            ITenantAppService service) =>
        {
            var dto = await service.DeactivateAsync(id);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("DeactivateTenant")
        .WithSummary("Deactivates a tenant.")
        .Produces<ApiResponse<TenantDto>>();

        return endpoints;
    }
}
