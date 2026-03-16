namespace LHA.BlazorWasm.UI.Navigation;

/// <summary>
/// Defines a menu item in the navigation system.
/// Supports nesting, permissions, badges, and dynamic registration.
/// </summary>
public sealed class MenuItemDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public string? Permission { get; set; }
    public string? Badge { get; set; }
    public string? BadgeStyle { get; set; }
    public int Order { get; set; }
    public string? Group { get; set; }
    public bool IsDivider { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? Target { get; set; }
    public List<MenuItemDefinition> Children { get; set; } = [];

    /// <summary>
    /// Custom metadata that modules can attach for extensibility.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}
