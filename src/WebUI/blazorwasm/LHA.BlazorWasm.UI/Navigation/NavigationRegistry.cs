namespace LHA.BlazorWasm.UI.Navigation;

/// <summary>
/// Default implementation of the navigation registry.
/// Thread-safe for concurrent module registration at startup.
/// </summary>
public sealed class NavigationRegistry : INavigationRegistry
{
    private readonly Dictionary<string, List<MenuItemDefinition>> _sections = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<RouteDefinition> _routes = [];
    private readonly object _lock = new();

    public event Action? OnNavigationChanged;

    public IReadOnlyList<MenuItemDefinition> GetMenuItems(string section = "main")
    {
        lock (_lock)
        {
            if (_sections.TryGetValue(section, out var items))
            {
                return items.OrderBy(x => x.Order).ToList().AsReadOnly();
            }
            return Array.Empty<MenuItemDefinition>();
        }
    }

    public RouteDefinition? GetRoute(string path)
    {
        lock (_lock)
        {
            return _routes.FirstOrDefault(r =>
                string.Equals(r.Path, path, StringComparison.OrdinalIgnoreCase));
        }
    }

    public IReadOnlyList<RouteDefinition> GetAllRoutes()
    {
        lock (_lock)
        {
            return _routes.AsReadOnly();
        }
    }

    public void RegisterMenuItems(string section, IEnumerable<MenuItemDefinition> items)
    {
        lock (_lock)
        {
            if (!_sections.TryGetValue(section, out var list))
            {
                list = [];
                _sections[section] = list;
            }
            list.AddRange(items);
        }
        OnNavigationChanged?.Invoke();
    }

    public void RegisterRoutes(IEnumerable<RouteDefinition> routes)
    {
        lock (_lock)
        {
            _routes.AddRange(routes);
        }
        OnNavigationChanged?.Invoke();
    }
}
