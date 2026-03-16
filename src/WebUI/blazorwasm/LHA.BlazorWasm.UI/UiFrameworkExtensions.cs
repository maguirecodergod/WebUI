using LHA.BlazorWasm.UI.Dashboard;
using LHA.BlazorWasm.UI.Extensions;
using LHA.BlazorWasm.UI.Modules;
using LHA.BlazorWasm.UI.Navigation;
using LHA.BlazorWasm.UI.Permissions;
using LHA.BlazorWasm.UI.Services;
using LHA.BlazorWasm.UI.State;
using LHA.BlazorWasm.UI.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.UI;

/// <summary>
/// Extension methods for registering the UI framework in DI.
/// </summary>
public static class UiFrameworkExtensions
{
    /// <summary>
    /// Registers all UI framework services.
    /// </summary>
    public static IServiceCollection AddLhaUiFramework(this IServiceCollection services)
    {
        // Navigation
        services.AddSingleton<INavigationRegistry, NavigationRegistry>();

        // Permissions
        services.AddSingleton<PermissionService>();
        services.AddSingleton<IPermissionService>(sp => sp.GetRequiredService<PermissionService>());

        // Tenant
        services.AddSingleton<ITenantContext, TenantContext>();

        // Modules
        services.AddSingleton<ModuleRegistry>();

        // Dashboard
        services.AddSingleton<IDashboardRegistry, DashboardRegistry>();

        // Extensions
        services.AddSingleton<IExtensionRegistry, ExtensionRegistry>();

        // State
        services.AddSingleton<UiStateStore>();

        // Services
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ICommandPaletteService, CommandPaletteService>();
        services.AddSingleton<INotificationService, NotificationService>();

        return services;
    }

    /// <summary>
    /// Register a UI module.
    /// </summary>
    public static IServiceCollection AddUiModule<TModule>(this IServiceCollection services)
        where TModule : class, IUiModule, new()
    {
        services.AddSingleton<IUiModule, TModule>();
        return services;
    }

    /// <summary>
    /// Initialize all registered UI modules.
    /// Call this after building the host.
    /// </summary>
    public static void InitializeUiModules(this IServiceProvider services)
    {
        var moduleRegistry = services.GetRequiredService<ModuleRegistry>();
        var modules = services.GetServices<IUiModule>();

        foreach (var module in modules)
        {
            moduleRegistry.RegisterModule(module);
        }

        moduleRegistry.InitializeModules();

        // Grant all permissions in development (remove in production)
        var permissionService = services.GetRequiredService<PermissionService>();
        permissionService.GrantAll();
    }
}
