namespace LHA.BlazorWasm.Components.RichTextEditor.Models;

/// <summary>
/// Configuration options for the RichTextEditor component.
/// </summary>
public class EditorOptions
{
    /// <summary>
    /// Height of the editor content area (e.g., "400px", "50vh").
    /// </summary>
    public string Height { get; set; } = "350px";

    /// <summary>
    /// Width of the editor (e.g., "100%", "800px").
    /// </summary>
    public string Width { get; set; } = "100%";

    /// <summary>
    /// Placeholder text shown when editor is empty.
    /// </summary>
    public string Placeholder { get; set; } = "Start typing...";

    /// <summary>
    /// If true, the editor is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// If true, shows the toolbar.
    /// </summary>
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// If true, shows the status bar at the bottom.
    /// </summary>
    public bool ShowStatusBar { get; set; } = true;

    /// <summary>
    /// Toolbar layout configuration.
    /// </summary>
    public ToolbarConfig ToolbarConfig { get; set; } = ToolbarConfig.Default;

    /// <summary>
    /// Debounce interval in milliseconds for content change events.
    /// </summary>
    public int DebounceMs { get; set; } = 300;
}
