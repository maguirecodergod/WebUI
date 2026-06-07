using Microsoft.AspNetCore.Routing;

namespace LHA.Mega.HttpApi;

/// <summary>
/// Maps endpoints for Mega management.
/// </summary>
public static class MegaEndpoints
{
    /// <summary>
    /// Maps endpoints for Mega management.
    /// Includes endpoints for Mega Account management.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapMegaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMegaAccountEndpoints();
        return endpoints;
    }
}
