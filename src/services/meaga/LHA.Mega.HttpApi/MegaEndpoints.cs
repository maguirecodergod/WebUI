using Microsoft.AspNetCore.Routing;

namespace LHA.Mega.HttpApi;

public static class MegaEndpoints
{
    public static IEndpointRouteBuilder MapMegaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMegaAccountEndpoints();
        return endpoints;
    }
}
