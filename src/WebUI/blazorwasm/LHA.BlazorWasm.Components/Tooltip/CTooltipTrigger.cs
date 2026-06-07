namespace LHA.BlazorWasm.Components.Tooltip;

/// <summary>
/// Defines the local DOM event interactions that will display the tooltip.
/// </summary>
public enum CTooltipTrigger
{
    /// <summary>
    /// 0 - Hover
    /// </summary>
    Hover,
    /// <summary>
    /// 1 - Click
    /// </summary>
    Click,
    /// <summary>
    /// 2 - Focus
    /// </summary>
    Focus,
    /// <summary>
    /// 3 - Manual
    /// </summary>
    Manual
}
