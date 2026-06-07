namespace LHA.BlazorWasm.Components.Skeleton;

/// <summary>
/// Defines the visual variant for the Skeleton component.
/// </summary>
public enum CSkeletonVariant
{
    /// <summary>
    /// 0 - Text: A rectangular block representing text lines. Typically adapts to the font height.
    /// </summary>
    Text,

    /// <summary>
    /// 1 - Rectangular: A sharp-edged rectangular block. Useful for cards or images.
    /// </summary>
    Rectangular,

    /// <summary>
    /// 2 - Circular: A perfectly circular shape. Useful for avatars.
    /// </summary>
    Circular,

    /// <summary>
    /// 3 - Rounded: A rectangle with rounded corners. Useful for buttons or custom rounded shapes.
    /// </summary>
    Rounded
}
