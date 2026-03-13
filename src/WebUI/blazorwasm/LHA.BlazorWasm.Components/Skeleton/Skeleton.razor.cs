using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Skeleton;

/// <summary>
/// A reusable enterprise-grade Skeleton loading component.
/// It displays placeholder shapes while data is loading.
/// 
/// Example usage:
/// 
/// Basic:
/// <Skeleton Width="200px" Height="20px" />
/// 
/// Text block:
/// <Skeleton Variant="SkeletonVariant.Text" Count="4" />
/// 
/// Avatar:
/// <Skeleton Variant="SkeletonVariant.Circular"
///           Width="48px"
///           Height="48px" />
/// 
/// Card placeholder:
/// <Skeleton Variant="SkeletonVariant.Rectangular"
///           Width="100%"
///           Height="200px" />
///           
/// Wrapping Content:
/// <Skeleton IsLoading="true">
///     <UserCard />
/// </Skeleton>
/// </summary>
public partial class Skeleton : ComponentBase
{
    /// <summary>
    /// Gets or sets the visual variant of the skeleton. Default is Text.
    /// </summary>
    [Parameter] public SkeletonVariant Variant { get; set; } = SkeletonVariant.Text;

    /// <summary>
    /// Gets or sets the animation type. Default is Wave.
    /// </summary>
    [Parameter] public SkeletonAnimation Animation { get; set; } = SkeletonAnimation.Wave;

    /// <summary>
    /// Gets or sets the CSS width of the skeleton.
    /// Examples: "100%", "250px", "4rem".
    /// </summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the CSS height of the skeleton.
    /// Required primarily for Rectangular and Circular variants. Text variants normally adapt to font size.
    /// </summary>
    [Parameter] public string? Height { get; set; }

    /// <summary>
    /// Gets or sets the number of skeleton elements to render. Useful for rendering multiple text lines.
    /// </summary>
    [Parameter] public int Count { get; set; } = 1;

    /// <summary>
    /// Gets or sets additional custom CSS classes.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets additional inline CSS styles.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Used when the component wraps real content.
    /// When true, renders the skeleton. When false, renders the ChildContent.
    /// </summary>
    [Parameter] public bool IsLoading { get; set; } = true;

    /// <summary>
    /// Explictly defined child content to render when IsLoading is false.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Generates the necessary CSS classes for an individual skeleton item.
    /// </summary>
    private string GetCssClass()
    {
        var classes = new List<string> { "base-skeleton" };

        classes.Add($"skeleton-{Variant.ToString().ToLowerInvariant()}");
        classes.Add($"skeleton-{Animation.ToString().ToLowerInvariant()}");

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Generates the necessary inline styles for an individual skeleton item.
    /// </summary>
    private string GetCssStyle()
    {
        var styles = new List<string>();

        if (!string.IsNullOrWhiteSpace(Width))
        {
            styles.Add($"width: {Width}");
        }

        if (!string.IsNullOrWhiteSpace(Height))
        {
            styles.Add($"height: {Height}");
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            styles.Add(Style);
        }

        return string.Join("; ", styles);
    }
}
