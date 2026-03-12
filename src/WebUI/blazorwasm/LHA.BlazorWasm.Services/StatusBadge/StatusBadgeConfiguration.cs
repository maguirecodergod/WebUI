using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.StatusBadge;

public static class StatusBadgeConfiguration
{
    /// <summary>
    /// Registers the core Status Badge services.
    /// </summary>
    public static IServiceCollection AddStatusBadgeServices(this IServiceCollection services)
    {
        services.AddSingleton<IStatusBadgeService, StatusBadgeService>();
        return services;
    }

    /// <summary>
    /// Helper to register mappings for a specific module or feature.
    /// This should be called from the module's initialization logic.
    /// </summary>
    public static void RegisterBadgeMappings<TEnum>(this IServiceProvider serviceProvider, Action<StatusBadgeMappingBuilder<TEnum>> builder) 
        where TEnum : struct, Enum
    {
        var badgeService = serviceProvider.GetRequiredService<IStatusBadgeService>();
        badgeService.Register(builder);
    }
}
