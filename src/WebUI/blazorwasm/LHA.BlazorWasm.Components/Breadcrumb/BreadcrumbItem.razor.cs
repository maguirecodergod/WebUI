using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Breadcrumb;

/// <summary>
/// A single item inside a <see cref="Breadcrumb"/> component.
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
public partial class BreadcrumbItem : LhaComponentBase, IDisposable
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

    // ──────────────────────────────────────────────
    // Cascading parent
    // ──────────────────────────────────────────────

    [CascadingParameter] private Breadcrumb? Parent { get; set; }

    [CascadingParameter(Name = "BreadcrumbSeparator")] 
    internal string Separator { get; set; } = "/";

    // ──────────────────────────────────────────────
    // Internal plumbing – set by the parent Breadcrumb
    // ──────────────────────────────────────────────

    /// <summary>Marks this item as the last one in the trail.</summary>
    internal bool IsLast { get; set; }

    // ──────────────────────────────────────────────
    // Lifecycle
    // ──────────────────────────────────────────────

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Parent?.Register(this);
        Parent?.NotifyChanged();
    }

    public override void Dispose()
    {
        base.Dispose();
        Parent?.Unregister(this);
        Parent?.NotifyChanged();
    }

    // ──────────────────────────────────────────────
    // Computed helpers
    // ──────────────────────────────────────────────

    internal bool IsLink => !string.IsNullOrWhiteSpace(Href) && !IsLast && !Disabled;

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

