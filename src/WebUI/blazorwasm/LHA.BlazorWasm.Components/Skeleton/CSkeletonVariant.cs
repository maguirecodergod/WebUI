namespace LHA.BlazorWasm.Components.Skeleton;

/// <summary>
/// Defines the visual variant for the Skeleton component.
/// </summary>
public enum CSkeletonVariant
{
    /// <summary>
    /// A rectangular block representing text lines. Typically adapts to the font height.
    /// </summary>
    Text,

    /// <summary>
    /// A sharp-edged rectangular block. Useful for cards or images.
    /// </summary>
    Rectangular,

    /// <summary>
    /// A perfectly circular shape. Useful for avatars.
    /// </summary>
    Circular,

    /// <summary>
    /// A rectangle with rounded corners. Useful for buttons or custom rounded shapes.
    /// </summary>
    Rounded
}
