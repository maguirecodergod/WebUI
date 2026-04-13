using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.Auth;

public static class AuthServiceExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
        services.AddSingleton<AuthTokenCache>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
        
        // This is necessary if we want to inject ApiAuthenticationStateProvider directly
        services.AddScoped(sp => (ApiAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
        
        return services;
    }
}
