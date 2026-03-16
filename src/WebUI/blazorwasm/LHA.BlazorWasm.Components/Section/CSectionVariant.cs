namespace LHA.BlazorWasm.Components.Section;

/// <summary>
/// Defines the visual variant for a <see cref="Section"/> component.
/// </summary>
public enum CSectionVariant
{
    /// <summary>
    /// Default bordered card style.
    /// </summary>
    Default,

    /// <summary>
    /// Subtle style with minimal border.
    /// </summary>
    Subtle,

    /// <summary>
    /// Flush style with no outer border or padding — ideal for nesting.
    /// </summary>
    Flush
}
