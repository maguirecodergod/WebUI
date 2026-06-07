namespace LHA.BlazorWasm.Components.Sidebar.Models;

/// <summary>
/// Represents the visual display state of the sidebar component.
/// </summary>
public enum CSidebarState
{
    /// <summary>
    /// 0 - Expanded: The sidebar is fully expanded showing icons and text labels. Default state on desktop viewports.
    /// </summary>
    Expanded,

    /// <summary>
    /// 1 - Mini: The sidebar is collapsed to a narrow "mini" rail showing icons only. Default state on tablet viewports. Expands on hover.
    /// </summary>
    Mini,

    /// <summary>
    /// 2 - Hidden: The sidebar is completely hidden from view. Default state on mobile viewports (shown as off-canvas drawer on demand).
    /// </summary>
    Hidden
}
