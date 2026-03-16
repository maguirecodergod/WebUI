namespace LHA.BlazorWasm.UI.Dashboard;

/// <summary>
/// Registry for dashboard widgets.
/// Modules register their widgets here, and the dashboard layout renders them.
/// </summary>
public interface IDashboardRegistry
{
    /// <summary>
    /// Register a widget definition.
    /// </summary>
    void RegisterWidget(WidgetDefinition widget);

    /// <summary>
    /// Get all registered widgets.
    /// </summary>
    IReadOnlyList<WidgetDefinition> GetWidgets();

    /// <summary>
    /// Get a widget by ID.
    /// </summary>
    WidgetDefinition? GetWidget(string id);

    /// <summary>
    /// Event raised when widgets change.
    /// </summary>
    event Action? OnWidgetsChanged;
}

/// <summary>
/// Default implementation of the dashboard registry.
/// </summary>
public sealed class DashboardRegistry : IDashboardRegistry
{
    private readonly List<WidgetDefinition> _widgets = [];

    public event Action? OnWidgetsChanged;

    public void RegisterWidget(WidgetDefinition widget)
    {
        _widgets.Add(widget);
        OnWidgetsChanged?.Invoke();
    }

    public IReadOnlyList<WidgetDefinition> GetWidgets()
    {
        return _widgets.OrderBy(w => w.Order).ToList().AsReadOnly();
    }

    public WidgetDefinition? GetWidget(string id)
    {
        return _widgets.FirstOrDefault(w =>
            string.Equals(w.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
