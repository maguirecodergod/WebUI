using LHA.BlazorWasm.Services.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Components;

public static class ComponentExtensions
{
    /// <summary>
    /// Registers BlazorWasm component library requirements, such as internal localization resources.
    /// </summary>
    public static IServiceCollection AddBlazorWasmComponents(this IServiceCollection services)
    {
        // Inject component-level translations into the global LocalizationService
        services.Configure<LocalizationOptions>(options =>
        {
            var componentResourcePath = "_content/LHA.BlazorWasm.Components/localization/{0}.json";
            
            if (!options.ResourcePaths.Contains(componentResourcePath))
            {
                options.ResourcePaths.Add(componentResourcePath);
            }
        });

        // Topbar Service
        services.AddScoped<Topbar.ITopbarService, Topbar.TopbarService>();

        return services;
    }
}
