using LHA.BlazorWasm.UI.Dashboard;
using LHA.BlazorWasm.UI.Extensions;
using LHA.BlazorWasm.UI.Navigation;

namespace LHA.BlazorWasm.UI.Modules;

/// <summary>
/// Discovers and initializes all registered UI modules.
/// Orchestrates the registration of menus, routes, widgets, and extensions.
/// </summary>
public sealed class ModuleRegistry
{
    private readonly INavigationRegistry _navigationRegistry;
    private readonly IDashboardRegistry _dashboardRegistry;
    private readonly IExtensionRegistry _extensionRegistry;
    private readonly List<IUiModule> _modules = [];
    private bool _initialized;

    public ModuleRegistry(
        INavigationRegistry navigationRegistry,
        IDashboardRegistry dashboardRegistry,
        IExtensionRegistry extensionRegistry)
    {
        _navigationRegistry = navigationRegistry;
        _dashboardRegistry = dashboardRegistry;
        _extensionRegistry = extensionRegistry;
    }

    /// <summary>
    /// Register a UI module.
    /// </summary>
    public void RegisterModule(IUiModule module)
    {
        _modules.Add(module);
    }

    /// <summary>
    /// Register a UI module by type.
    /// </summary>
    public void RegisterModule<TModule>() where TModule : IUiModule, new()
    {
        _modules.Add(new TModule());
    }

    /// <summary>
    /// Initialize all registered modules.
    /// Must be called after all modules are registered.
    /// </summary>
    public void InitializeModules()
    {
        if (_initialized) return;

        var orderedModules = _modules.OrderBy(m => m.Order).ToList();

        foreach (var module in orderedModules)
        {
            var builder = new UiModuleBuilder();
            module.Configure(builder);

            // Register main navigation
            var mainMenuItems = builder.MainNavigation.GetMenuItems();
            if (mainMenuItems.Count > 0)
            {
                _navigationRegistry.RegisterMenuItems("main", mainMenuItems);
            }

            var mainRoutes = builder.MainNavigation.GetRoutes();
            if (mainRoutes.Count > 0)
            {
                _navigationRegistry.RegisterRoutes(mainRoutes);
            }

            // Register admin navigation
            var adminMenuItems = builder.AdminNavigation.GetMenuItems();
            if (adminMenuItems.Count > 0)
            {
                _navigationRegistry.RegisterMenuItems("admin", adminMenuItems);
            }

            var adminRoutes = builder.AdminNavigation.GetRoutes();
            if (adminRoutes.Count > 0)
            {
                _navigationRegistry.RegisterRoutes(adminRoutes);
            }

            // Register widgets
            foreach (var widget in builder.Widgets)
            {
                _dashboardRegistry.RegisterWidget(widget);
            }

            // Register extensions
            foreach (var (slotName, componentType) in builder.ExtensionSlots)
            {
                _extensionRegistry.RegisterExtension(slotName, componentType);
            }
        }

        _initialized = true;
    }

    /// <summary>
    /// Get all registered modules.
    /// </summary>
    public IReadOnlyList<IUiModule> GetModules() => _modules.AsReadOnly();
}
