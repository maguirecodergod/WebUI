using LHA.Ddd.Application;
using LHA.Mega.Application.Contracts.Account;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using P = LHA.Mega.Application.Contracts.Permissions.MegaPermissions;

namespace LHA.Mega.HttpApi;

public static class MegaAccountEndpoints
{
    public static IEndpointRouteBuilder MapMegaAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Mega", "/api/v{version:apiVersion}/mega/accounts")
            .WithTags("Mega - Accounts")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, IMegaAccountAppService service, CancellationToken ct) =>
        {
            var dto = await service.GetAsync(id, ct);
            return Results.Ok(ApiResponse<MegaAccountDto>.Ok(dto));
        })
        .RequireAuthorization(P.MegaAccountManagement.Read)
        .WithName("GetMegaAccount")
        .WithSummary("Gets a mega account by ID.");

        group.MapGet("/", async (
            [AsParameters] GetMegaAccountsInput input,
            IMegaAccountAppService service, CancellationToken ct) =>
        {
            var result = await service.GetListAsync(input, ct);
            return Results.Ok(ApiResponse<PagedResultDto<MegaAccountDto>>.Ok(result));
        })
        .RequireAuthorization(P.MegaAccountManagement.Read)
        .WithName("GetMegaAccountList")
        .WithSummary("Gets a paged list of mega accounts.");

        group.MapPost("/", async (CreateMegaAccountInput input, IMegaAccountAppService service, CancellationToken ct) =>
        {
            var dto = await service.CreateAsync(input, ct);
            return Results.Created($"/api/mega/accounts/{dto.Id}", ApiResponse<MegaAccountDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.MegaAccountManagement.Create)
        .WithName("CreateMegaAccount")
        .WithSummary("Creates a new mega account.");

        group.MapPut("/{id:guid}", async (Guid id, UpdateMegaAccountInput input, IMegaAccountAppService service, CancellationToken ct) =>
        {
            var dto = await service.UpdateAsync(id, input, ct);
            return Results.Ok(ApiResponse<MegaAccountDto>.Ok(dto));
        })
        .RequireAuthorization(P.MegaAccountManagement.Update)
        .WithName("UpdateMegaAccount")
        .WithSummary("Updates an existing mega account.");

        group.MapDelete("/{id:guid}", async (Guid id, IMegaAccountAppService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(P.MegaAccountManagement.Delete)
        .WithName("DeleteMegaAccount")
        .WithSummary("Deletes a mega account.");

        return endpoints;
    }
}
