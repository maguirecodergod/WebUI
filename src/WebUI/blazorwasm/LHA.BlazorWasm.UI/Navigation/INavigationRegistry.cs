namespace LHA.BlazorWasm.UI.Navigation;

/// <summary>
/// Central registry for all navigation menus and routes.
/// Modules register their menus here during startup.
/// </summary>
public interface INavigationRegistry
{
    /// <summary>
    /// Get all registered menu items for a specific section (e.g., "main", "admin", "settings").
    /// Items are returned sorted by Order, with permission-filtered items excluded.
    /// </summary>
    IReadOnlyList<MenuItemDefinition> GetMenuItems(string section = "main");

    /// <summary>
    /// Get route definition by path.
    /// </summary>
    RouteDefinition? GetRoute(string path);

    /// <summary>
    /// Get all registered routes.
    /// </summary>
    IReadOnlyList<RouteDefinition> GetAllRoutes();

    /// <summary>
    /// Register menu items for a specific section.
    /// </summary>
    void RegisterMenuItems(string section, IEnumerable<MenuItemDefinition> items);

    /// <summary>
    /// Register routes.
    /// </summary>
    void RegisterRoutes(IEnumerable<RouteDefinition> routes);

    /// <summary>
    /// Event raised when navigation items change (e.g., after module registration).
    /// </summary>
    event Action? OnNavigationChanged;
}
