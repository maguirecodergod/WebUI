namespace LHA.BlazorWasm.Components.Tabs;

/// <summary>
/// Defines the position of the tab header strip relative to the content panel.
/// </summary>
public enum CTabPosition
{
    /// <summary>
    /// 0 - Top: Tab headers appear above the content (default).
    /// </summary>
    Top,

    /// <summary>
    /// 1 - Bottom: Tab headers appear below the content.
    /// </summary>
    Bottom,

    /// <summary>
    /// 2 - Left: Tab headers appear to the left of the content.
    /// </summary>
    Left,

    /// <summary>
    /// 3 - Right: Tab headers appear to the right of the content.
    /// </summary>
    Right
}
