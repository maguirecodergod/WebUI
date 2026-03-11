using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// DI Extensions for seamless generic service registration targeting builder.Services blocks.
/// </summary>
public static class ToastExtensions
{
    /// <summary>
    /// Adds robust Toast pub/sub infrastructure singleton scope natively to the pipeline.
    /// Toasts must maintain cross-component queue visibility via Scoped singleton memory models in Wasm.
    /// </summary>
    public static IServiceCollection AddToastService(this IServiceCollection services)
    {
        services.AddScoped<IToastService, ToastService>();
        return services;
    }
}
