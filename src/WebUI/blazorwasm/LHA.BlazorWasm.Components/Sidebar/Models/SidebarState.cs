namespace LHA.BlazorWasm.Components.Sidebar.Models;

/// <summary>
/// Represents the visual display state of the sidebar component.
/// </summary>
public enum SidebarState
{
    /// <summary>
    /// The sidebar is fully expanded showing icons and text labels.
    /// Default state on desktop viewports.
    /// </summary>
    Expanded,

    /// <summary>
    /// The sidebar is collapsed to a narrow "mini" rail showing icons only.
    /// Default state on tablet viewports. Expands on hover.
    /// </summary>
    Mini,

    /// <summary>
    /// The sidebar is completely hidden from view.
    /// Default state on mobile viewports (shown as off-canvas drawer on demand).
    /// </summary>
    Hidden
}
