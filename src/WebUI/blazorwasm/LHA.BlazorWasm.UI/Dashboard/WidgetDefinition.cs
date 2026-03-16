namespace LHA.BlazorWasm.UI.Dashboard;

/// <summary>
/// Defines a dashboard widget.
/// </summary>
public sealed class WidgetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Permission { get; set; }
    public Type? ComponentType { get; set; }
    public int DefaultColumn { get; set; }
    public int DefaultRow { get; set; }
    public int ColumnSpan { get; set; } = 1;
    public int RowSpan { get; set; } = 1;
    public bool IsResizable { get; set; } = true;
    public bool IsDraggable { get; set; } = true;
    public int Order { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = [];
}
