namespace LHA.BlazorWasm.Components.Section;

/// <summary>
/// Defines the visual variant for a <see cref="Section"/> component.
/// </summary>
public enum CSectionVariant
{
    /// <summary>
    /// 0 - Default: Default bordered card style.
    /// </summary>
    Default,

    /// <summary>
    /// 1 - Subtle: Subtle style with minimal border.
    /// </summary>
    Subtle,

    /// <summary>
    /// 2 - Flush: Flush style with no outer border or padding — ideal for nesting.
    /// </summary>
    Flush
}
