namespace LHA.BlazorWasm.UI.Navigation;

/// <summary>
/// Route metadata for the navigation system.
/// Associates routes with layout types, page titles, breadcrumb info, and permissions.
/// </summary>
public sealed class RouteDefinition
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? BreadcrumbTitle { get; set; }
    public string? ParentRoute { get; set; }
    public string? Permission { get; set; }
    public Type? LayoutType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];
}
