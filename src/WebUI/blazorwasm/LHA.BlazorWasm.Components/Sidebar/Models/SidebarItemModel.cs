namespace LHA.BlazorWasm.Components.Sidebar.Models;

/// <summary>
/// Represents a single node in the sidebar navigation tree.
/// Supports infinite levels of nesting via the <see cref="Children"/> collection.
/// </summary>
public sealed class SidebarItemModel
{
    /// <summary>
    /// Unique identifier for this sidebar item. Used for tracking expand/collapse state
    /// and deduplication during rendering.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// The localization key for the item's display title.
    /// Resolved at render time via <c>ILocalizationService.L(TitleKey)</c>.
    /// When no translation is found, the key itself is displayed as a fallback.
    /// </summary>
    public string TitleKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional SVG icon markup or CSS icon class rendered beside the title.
    /// Accepts raw SVG strings (e.g., <c>&lt;svg&gt;...&lt;/svg&gt;</c>) or icon class names.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The navigation URI this item routes to when clicked.
    /// Leave <c>null</c> for group/header-only items that only expand children.
    /// Relative paths (e.g., <c>/dashboard</c>) are matched against the current route.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Optional CSS class to apply to this specific item's container for custom styling.
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// An optional badge value displayed as a pill/counter on the item (e.g., unread count).
    /// </summary>
    public string? Badge { get; set; }

    /// <summary>
    /// CSS class for the badge styling (e.g., "badge--danger", "badge--info").
    /// </summary>
    public string? BadgeCssClass { get; set; }

    /// <summary>
    /// Whether this item is currently disabled and non-interactive.
    /// Disabled items are rendered with reduced opacity and pointer-events disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Whether this item is visible. Defaults to <c>true</c>.
    /// Hidden items are not rendered in the DOM at all.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Whether this item acts as a visual section divider/separator.
    /// When <c>true</c>, the item renders as a horizontal line instead of a clickable nav link.
    /// </summary>
    public bool IsDivider { get; set; }

    /// <summary>
    /// Determines the link target attribute (e.g., "_blank" for new tab).
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Recursive children collection. Allows N-level deep nesting.
    /// An empty or <c>null</c> list indicates this is a leaf node.
    /// </summary>
    public List<SidebarItemModel> Children { get; set; } = new();

    /// <summary>
    /// Gets whether this node has any visible children.
    /// </summary>
    public bool HasChildren => Children.Any(c => c.IsVisible);

    /// <summary>
    /// Optional integer for custom sort ordering within its parent's children list.
    /// Lower values appear first. Defaults to <c>0</c>.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Optional permission requirement for this item.
    /// If set, the item will only be visible if the user has this permission.
    /// </summary>
    public string? RequiredPermission { get; set; }

    /// <summary>
    /// Optional match mode for route-based active state detection.
    /// When <see cref="NavLinkMatch.All"/>, only exact matches highlight.
    /// When <see cref="NavLinkMatch.Prefix"/>, prefix matches also highlight (default).
    /// </summary>
    public CNavLinkMatchMode MatchMode { get; set; } = CNavLinkMatchMode.Prefix;
}

/// <summary>
/// Defines how the sidebar matches the current URL to determine the active item.
/// </summary>
public enum CNavLinkMatchMode
{
    /// <summary>Matches when the current URI starts with the item's Href.</summary>
    Prefix,

    /// <summary>Matches only when the current URI exactly equals the item's Href.</summary>
    Exact
}
