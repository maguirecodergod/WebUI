using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using P = LHA.Shared.Contracts.Identity.IdentityPermissions;

namespace LHA.Identity.HttpApi;

/// <summary>
/// Maps claim type and security log endpoints.
/// </summary>
public static class ClaimTypeAndSecurityLogEndpoints
{
    public static IEndpointRouteBuilder MapClaimTypeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity/claim-types")
            .WithTags("Identity - Claim Types")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetClaimTypesInput input,
            IIdentityClaimTypeAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<IdentityClaimTypeDto>>.Ok(result));
        })
        .RequireAuthorization(P.ClaimTypes.Read)
        .WithName("GetClaimTypes")
        .WithSummary("Returns a paged list of claim types.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IIdentityClaimTypeAppService service) =>
        {
            var dto = await service.GetAsync(id);
            return Results.Ok(ApiResponse<IdentityClaimTypeDto>.Ok(dto));
        })
        .RequireAuthorization(P.ClaimTypes.Read)
        .WithName("GetClaimType")
        .WithSummary("Gets a claim type by ID.");

        group.MapPost("/", async (
            CreateOrUpdateClaimTypeInput input,
            IIdentityClaimTypeAppService service) =>
        {
            var dto = await service.CreateAsync(input);
            return Results.Created($"/api/identity/claim-types/{dto.Id}",
                ApiResponse<IdentityClaimTypeDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.ClaimTypes.Create)
        .WithName("CreateClaimType")
        .WithSummary("Creates a new claim type.");

        group.MapPut("/{id:guid}", async (
            Guid id,
            CreateOrUpdateClaimTypeInput input,
            IIdentityClaimTypeAppService service) =>
        {
            var dto = await service.UpdateAsync(id, input);
            return Results.Ok(ApiResponse<IdentityClaimTypeDto>.Ok(dto));
        })
        .RequireAuthorization(P.ClaimTypes.Update)
        .WithName("UpdateClaimType")
        .WithSummary("Updates a claim type.");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IIdentityClaimTypeAppService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(P.ClaimTypes.Delete)
        .WithName("DeleteClaimType")
        .WithSummary("Deletes a claim type.");

        return endpoints;
    }

    public static IEndpointRouteBuilder MapSecurityLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity/security-logs")
            .WithTags("Identity - Security Logs")
            .RequireAuthorization();

        group.MapGet("/", async (
            [AsParameters] GetSecurityLogsInput input,
            IIdentitySecurityLogAppService service) =>
        {
            var result = await service.GetListAsync(input);
            return Results.Ok(ApiResponse<PagedResultDto<IdentitySecurityLogDto>>.Ok(result));
        })
        .RequireAuthorization(P.SecurityLogs.Read)
        .WithName("GetSecurityLogs")
        .WithSummary("Returns a paged list of security logs.");

        return endpoints;
    }
}
