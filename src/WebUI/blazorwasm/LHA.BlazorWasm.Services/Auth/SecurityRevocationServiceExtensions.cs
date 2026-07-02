using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.Auth;

public static class SecurityRevocationServiceExtensions
{
    public static IServiceCollection AddSecurityRevocationUi(this IServiceCollection services)
    {
        services.AddScoped<ISecurityRevocationUiService, SecurityRevocationUiService>();
        return services;
    }
}
