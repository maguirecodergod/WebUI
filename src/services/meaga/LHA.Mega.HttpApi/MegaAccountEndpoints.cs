using LHA.Ddd.Application;
using LHA.Mega.Application.Contracts.Account;
using LHA.Mega.Application.Contracts.Constants.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using P = LHA.Mega.Application.Contracts.Constants.Permissions.MegaPermissions;

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
        .RequireAuthorization(P.Accounts.Read)
        .WithName("GetMegaAccount")
        .WithSummary("Gets a mega account by ID.");

        group.MapGet("/", async (
            string? filter, bool? isActive, string? sorting,
            int skipCount, int maxResultCount,
            IMegaAccountAppService service, CancellationToken ct) =>
        {
            var result = await service.GetListAsync(filter, isActive, sorting, skipCount, maxResultCount, ct);
            return Results.Ok(ApiResponse<PagedResultDto<MegaAccountDto>>.Ok(result));
        })
        .RequireAuthorization(P.Accounts.Read)
        .WithName("GetMegaAccountList")
        .WithSummary("Gets a paged list of mega accounts.");

        group.MapPost("/", async (CreateMegaAccountInput input, IMegaAccountAppService service, CancellationToken ct) =>
        {
            var dto = await service.CreateAsync(input, ct);
            return Results.Created($"/api/mega/accounts/{dto.Id}", ApiResponse<MegaAccountDto>.Ok(dto, 201));
        })
        .RequireAuthorization(P.Accounts.Create)
        .WithName("CreateMegaAccount")
        .WithSummary("Creates a new mega account.");

        group.MapPut("/{id:guid}", async (Guid id, UpdateMegaAccountInput input, IMegaAccountAppService service, CancellationToken ct) =>
        {
            var dto = await service.UpdateAsync(id, input, ct);
            return Results.Ok(ApiResponse<MegaAccountDto>.Ok(dto));
        })
        .RequireAuthorization(P.Accounts.Update)
        .WithName("UpdateMegaAccount")
        .WithSummary("Updates an existing mega account.");

        group.MapDelete("/{id:guid}", async (Guid id, IMegaAccountAppService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        })
        .RequireAuthorization(P.Accounts.Delete)
        .WithName("DeleteMegaAccount")
        .WithSummary("Deletes a mega account.");

        return endpoints;
    }
}
