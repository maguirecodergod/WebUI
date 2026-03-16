using LHA.Ddd.Application;
using LHA.TenantManagement.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.TenantManagement.Application.Contracts.TenantManagementPermissions;

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
        var group = endpoints.MapGroup("/api/tenants")
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
        .WithSummary("Returns a filtered, paged list of tenants.");

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
        .WithSummary("Gets a tenant by its unique identifier.");

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
        .WithSummary("Finds a tenant by display name (case-insensitive).");

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
        .WithSummary("Creates a new tenant.");

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
        .WithSummary("Updates a tenant (name). Requires concurrency stamp.");

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
        .WithSummary("Soft-deletes a tenant.");

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
        .WithSummary("Returns all connection strings for a tenant.");

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
        .WithSummary("Adds or updates a named connection string on the tenant.");

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
        .WithSummary("Removes a named connection string from the tenant.");

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
        .WithSummary("Activates a tenant.");

        group.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            ITenantAppService service) =>
        {
            var dto = await service.DeactivateAsync(id);
            return Results.Ok(ApiResponse<TenantDto>.Ok(dto));
        })
        .RequireAuthorization(P.Tenants.Update)
        .WithName("DeactivateTenant")
        .WithSummary("Deactivates a tenant.");

        return endpoints;
    }
}
