using LHA.Account.Application.Contracts.Permissions;
using LHA.Ddd.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LHA.Account.HttpApi;

/// <summary>
/// Internal endpoints for inter-service communication (excluded from OpenAPI documentation).
/// </summary>
public static class InternalEndpoints
{
    /// <summary>
    /// Maps internal-only endpoints that are not exposed via Scalar/Swagger.
    /// </summary>
    public static IEndpointRouteBuilder MapInternalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedGroup("Account", "/api/v{version:apiVersion}/internal")
            .WithTags("Internal")
            .ExcludeFromDescription();

        group.MapPost("/permissions/register", async (
            RegisterServicePermissionsInput input,
            IPermissionRegistrationService service,
            CancellationToken ct) =>
        {
            await service.RegisterAsync(input, ct);
            return Results.Ok(ApiResponse<string>.Ok("Permissions registered successfully."));
        })
        .AllowAnonymous()
        .WithName("RegisterServicePermissions")
        .WithSummary("Registers permissions from an external microservice.");

        return endpoints;
    }
}
