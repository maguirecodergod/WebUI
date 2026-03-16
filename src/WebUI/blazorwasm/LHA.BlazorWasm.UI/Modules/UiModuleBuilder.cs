using LHA.BlazorWasm.UI.Navigation;
using LHA.BlazorWasm.UI.Dashboard;
using LHA.BlazorWasm.UI.Extensions;

namespace LHA.BlazorWasm.UI.Modules;

/// <summary>
/// Fluent builder passed to each UI module during configuration.
/// Provides a unified API for registering menus, routes, widgets, and extensions.
/// </summary>
public sealed class UiModuleBuilder
{
    private readonly NavigationBuilder _mainNavBuilder = new();
    private readonly NavigationBuilder _adminNavBuilder = new();
    private readonly List<WidgetDefinition> _widgets = [];
    private readonly List<string> _permissions = [];
    private readonly Dictionary<string, Type> _extensionSlots = [];

    /// <summary>
    /// Register a main navigation menu item.
    /// </summary>
    public UiModuleBuilder AddMenu(string id, Action<MenuItemDefinition> configure)
    {
        _mainNavBuilder.AddMenu(id, configure);
        return this;
    }

    /// <summary>
    /// Register an admin navigation menu item.
    /// </summary>
    public UiModuleBuilder AddAdminMenu(string id, Action<MenuItemDefinition> configure)
    {
        _adminNavBuilder.AddMenu(id, configure);
        return this;
    }

    /// <summary>
    /// Register a navigation route.
    /// </summary>
    public UiModuleBuilder AddRoute(string path, Action<RouteDefinition> configure)
    {
        _mainNavBuilder.AddRoute(path, configure);
        return this;
    }

    /// <summary>
    /// Register a dashboard widget.
    /// </summary>
    public UiModuleBuilder AddWidget(string id, Action<WidgetDefinition> configure)
    {
        var widget = new WidgetDefinition { Id = id };
        configure(widget);
        _widgets.Add(widget);
        return this;
    }

    /// <summary>
    /// Register permission definitions that this module requires.
    /// </summary>
    public UiModuleBuilder AddPermissions(params string[] permissions)
    {
        _permissions.AddRange(permissions);
        return this;
    }

    /// <summary>
    /// Register a component for an extension slot.
    /// </summary>
    public UiModuleBuilder AddExtension(string slotName, Type componentType)
    {
        _extensionSlots[slotName] = componentType;
        return this;
    }

    // ── Internal Accessors ────────────────────────────────────────────
    internal NavigationBuilder MainNavigation => _mainNavBuilder;
    internal NavigationBuilder AdminNavigation => _adminNavBuilder;
    internal IReadOnlyList<WidgetDefinition> Widgets => _widgets.AsReadOnly();
    internal IReadOnlyList<string> Permissions => _permissions.AsReadOnly();
    internal IReadOnlyDictionary<string, Type> ExtensionSlots => _extensionSlots;
}
