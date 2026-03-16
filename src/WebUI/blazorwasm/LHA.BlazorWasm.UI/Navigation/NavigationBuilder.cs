namespace LHA.BlazorWasm.UI.Navigation;

/// <summary>
/// Fluent API for building navigation menus within modules.
/// </summary>
public sealed class NavigationBuilder
{
    private readonly List<MenuItemDefinition> _items = [];
    private readonly List<RouteDefinition> _routes = [];

    /// <summary>
    /// Add a menu item with fluent configuration.
    /// </summary>
    public NavigationBuilder AddMenu(string id, Action<MenuItemDefinition> configure)
    {
        var item = new MenuItemDefinition { Id = id };
        configure(item);
        _items.Add(item);
        return this;
    }

    /// <summary>
    /// Add a menu divider.
    /// </summary>
    public NavigationBuilder AddDivider(int order = 0)
    {
        _items.Add(new MenuItemDefinition { IsDivider = true, Order = order });
        return this;
    }

    /// <summary>
    /// Add a menu group header (non-clickable label).
    /// </summary>
    public NavigationBuilder AddGroup(string title, int order = 0)
    {
        _items.Add(new MenuItemDefinition
        {
            Id = $"group:{title.ToLowerInvariant().Replace(' ', '-')}",
            Title = title,
            Group = title,
            Order = order
        });
        return this;
    }

    /// <summary>
    /// Register a route with metadata.
    /// </summary>
    public NavigationBuilder AddRoute(string path, Action<RouteDefinition> configure)
    {
        var route = new RouteDefinition { Path = path };
        configure(route);
        _routes.Add(route);
        return this;
    }

    internal IReadOnlyList<MenuItemDefinition> GetMenuItems() => _items.AsReadOnly();
    internal IReadOnlyList<RouteDefinition> GetRoutes() => _routes.AsReadOnly();
}
