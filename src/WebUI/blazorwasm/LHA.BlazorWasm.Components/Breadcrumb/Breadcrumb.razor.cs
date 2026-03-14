using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Breadcrumb;

/// <summary>
/// Displays the current navigation hierarchy as a breadcrumb trail.
///
/// Supports two usage modes:
///
/// 1. Child-content markup (preferred):
/// <code>
/// &lt;Breadcrumb&gt;
///     &lt;BreadcrumbItem Text="Dashboard" Href="/" Icon="🏠" /&gt;
///     &lt;BreadcrumbItem Text="Users"     Href="/users" /&gt;
///     &lt;BreadcrumbItem Text="Details" /&gt;
/// &lt;/Breadcrumb&gt;
/// </code>
///
/// 2. Data-driven via <see cref="Items"/> parameter:
/// <code>
/// &lt;Breadcrumb Items="@breadcrumbItems" /&gt;
/// </code>
/// </summary>
public partial class Breadcrumb : ComponentBase
{
    // ──────────────────────────────────────────────
    // Parameters
    // ──────────────────────────────────────────────

    /// <summary>
    /// Child <see cref="BreadcrumbItem"/> components rendered as markup.
    /// Use this OR <see cref="Items"/>, not both.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Alternative data-driven list of items. Ignored when <see cref="ChildContent"/> is also set.
    /// </summary>
    [Parameter] public IEnumerable<BreadcrumbItemModel>? Items { get; set; }

    /// <summary>
    /// Separator string rendered between items. Defaults to <c>/</c>.
    /// </summary>
    [Parameter] public string Separator { get; set; } = "/";

    /// <summary>Additional CSS classes applied to the <c>&lt;nav&gt;</c> root.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Inline styles applied to the <c>&lt;nav&gt;</c> root.</summary>
    [Parameter] public string? Style { get; set; }

    // ──────────────────────────────────────────────
    // Internal state
    // ──────────────────────────────────────────────

    /// <summary>Holds the registered child items (markup mode).</summary>
    private readonly List<BreadcrumbItem> _childItems = new();

    // ──────────────────────────────────────────────
    // Child registration (markup mode)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Called by child <see cref="BreadcrumbItem"/> components to register
    /// themselves so the parent can mark the last one as active.
    /// </summary>
    internal void Register(BreadcrumbItem item)
    {
        if (!_childItems.Contains(item))
            _childItems.Add(item);
    }

    /// <summary>
    /// Called by child <see cref="BreadcrumbItem"/> components when they are disposed.
    /// </summary>
    internal void Unregister(BreadcrumbItem item)
    {
        _childItems.Remove(item);
    }

    /// <summary>
    /// Refreshes the <c>IsLast</c> flag on every registered child
    /// so only the final item gets the active style.
    /// Called by children after they register.
    /// </summary>
    internal void NotifyChanged()
    {
        for (var i = 0; i < _childItems.Count; i++)
            _childItems[i].IsLast = (i == _childItems.Count - 1);

        StateHasChanged();
    }

    // ──────────────────────────────────────────────
    // Computed helpers (data-driven mode)
    // ──────────────────────────────────────────────

    private bool UseChildContent => ChildContent is not null;

    private IReadOnlyList<BreadcrumbItemModel> DataItems =>
        Items?.ToList() ?? new List<BreadcrumbItemModel>();

    private string NavClass =>
        string.IsNullOrWhiteSpace(Class)
            ? "breadcrumb-nav"
            : $"breadcrumb-nav {Class}";
}
