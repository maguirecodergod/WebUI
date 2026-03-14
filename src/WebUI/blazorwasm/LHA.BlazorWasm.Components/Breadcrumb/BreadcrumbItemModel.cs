namespace LHA.BlazorWasm.Components.Breadcrumb;

/// <summary>
/// Represents a single item in the Breadcrumb navigation component.
/// Used when providing items via the <c>Items</c> parameter instead of child markup.
/// </summary>
public class BreadcrumbItemModel
{
    /// <summary>
    /// Gets or sets the display text for this breadcrumb item.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the navigation URL. When set, the item renders as a link.
    /// When null, the item renders as plain (active) text.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets an optional icon displayed before the item text.
    /// Accepts any string value — typically an emoji or an icon font class.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets whether this item is disabled (non-interactive).
    /// </summary>
    public bool Disabled { get; set; }
}
