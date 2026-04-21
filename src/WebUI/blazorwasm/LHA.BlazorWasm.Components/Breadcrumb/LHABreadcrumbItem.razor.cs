using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Breadcrumb;

/// <summary>
/// A single item inside a <see cref="LHABreadcrumb"/> component.
///
/// Renders as a <c>NavLink</c> when an <see cref="Href"/> is provided,
/// or as plain active text when <see cref="Href"/> is omitted (i.e., the
/// current/last page in the trail).
///
/// Example:
/// <code>
/// &lt;BreadcrumbItem Icon="🏠" Text="Dashboard" Href="/" /&gt;
/// &lt;BreadcrumbItem Text="Users" Href="/users" /&gt;
/// &lt;BreadcrumbItem Text="Details" /&gt;
/// </code>
/// </summary>
public partial class LHABreadcrumbItem : LHAComponentBase
{
    // ──────────────────────────────────────────────
    // Parameters
    // ──────────────────────────────────────────────

    /// <summary>Display label for this item.</summary>
    [Parameter] public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional navigation URL. When set the item renders as an anchor/NavLink.
    /// Leave null for the active (last) item.
    /// </summary>
    [Parameter] public string? Href { get; set; }

    /// <summary>
    /// Optional icon (emoji, Unicode symbol, or icon-font character)
    /// displayed before the text.
    /// </summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>
    /// When true, the item is rendered non-interactively and styled as disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// The parent <see cref="LHABreadcrumb"/> component.
    /// </summary>
    [CascadingParameter]
    private LHABreadcrumb? Parent { get; set; }

    /// <summary>
    /// The separator used between breadcrumb items.
    /// </summary>
    [CascadingParameter(Name = "BreadcrumbSeparator")]
    internal string Separator { get; set; } = "/";

    /// <summary>
    /// Marks this item as the last one in the trail.
    /// </summary>
    internal bool IsLast { get; set; }

    /// <summary>
    /// Initializes the component and registers it with the parent.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Parent?.Register(this);
        Parent?.NotifyChanged();
    }

    /// <summary>
    /// Disposes the component and unregisters it from the parent.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        Parent?.Unregister(this);
        Parent?.NotifyChanged();
    }

    /// <summary>
    /// Determines if the item is a link.
    /// </summary>
    internal bool IsLink => !string.IsNullOrWhiteSpace(Href) && !IsLast && !Disabled;

    /// <summary>
    /// Gets the CSS class for the item.
    /// </summary>
    internal string ItemCssClass
    {
        get
        {
            var list = new List<string> { "breadcrumb-item" };
            if (IsLast) list.Add("breadcrumb-active");
            if (Disabled) list.Add("breadcrumb-disabled");
            return string.Join(" ", list);
        }
    }
}

