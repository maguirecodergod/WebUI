using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Tabs;

/// <summary>
/// Declarative child component for <see cref="Tabs"/>.
///
/// Usage:
/// <code>
/// &lt;Tabs&gt;
///     &lt;TabItem Title="Info" Icon="bi bi-info-circle"&gt;
///         &lt;Content&gt;… your content …&lt;/Content&gt;
///     &lt;/TabItem&gt;
/// &lt;/Tabs&gt;
/// </code>
/// </summary>
public partial class TabItem : LHAComponentBase, IDisposable
{
    // ── Cascade ───────────────────────────────────────────────
    [CascadingParameter] private Tabs? Parent { get; set; }

    // ── Identity / Display ────────────────────────────────────

    /// <summary>Text label rendered inside the tab header button.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Bootstrap-icon class (e.g. "bi bi-house") rendered before the label.</summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>Optional short text rendered as a pill badge after the label (e.g. "3").</summary>
    [Parameter] public string? Badge { get; set; }

    // ── State ─────────────────────────────────────────────────

    /// <summary>When true, the tab header is grayed-out and not clickable.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>When true, a close (×) icon is shown inside the tab header.</summary>
    [Parameter] public bool Closable { get; set; }

    // ── Rendering ─────────────────────────────────────────────

    /// <summary>
    /// Body content rendered inside the tab panel when this tab is active.
    /// Use the named slot: &lt;Content&gt;…&lt;/Content&gt;
    /// </summary>
    [Parameter] public RenderFragment? Content { get; set; }

    // ── Styling ───────────────────────────────────────────────

    /// <summary>Extra CSS class(es) applied to this tab's header button.</summary>
    [Parameter] public string? HeaderClass { get; set; }

    /// <summary>Extra CSS class(es) applied to this tab's content panel div.</summary>
    [Parameter] public string? ContentClass { get; set; }

    // ── Internal definition ───────────────────────────────────
    private TabDefinition? _definition;

    protected override void OnInitialized()
    {
        if (Parent is null)
            throw new InvalidOperationException($"{nameof(TabItem)} must be used inside a {nameof(Tabs)} component.");

        _definition = new TabDefinition
        {
            Title = Title,
            Icon = Icon,
            Badge = Badge,
            Disabled = Disabled,
            Closable = Closable,
            Content = Content,
            HeaderClass = HeaderClass,
            ContentClass = ContentClass
        };

        Parent.RegisterTab(_definition);
    }

    protected override void OnParametersSet()
    {
        if (_definition is null) return;

        bool changed =
            _definition.Title != Title ||
            _definition.Icon != Icon ||
            _definition.Badge != Badge ||
            _definition.Disabled != Disabled ||
            _definition.Closable != Closable ||
            _definition.HeaderClass != HeaderClass ||
            _definition.ContentClass != ContentClass;

        _definition.Title = Title;
        _definition.Icon = Icon;
        _definition.Badge = Badge;
        _definition.Disabled = Disabled;
        _definition.Closable = Closable;
        _definition.Content = Content;
        _definition.HeaderClass = HeaderClass;
        _definition.ContentClass = ContentClass;

        if (changed)
        {
            Parent?.NotifyStateChanged();
        }
    }

    public override void Dispose()
    {
        if (_definition is not null)
            Parent?.UnregisterTab(_definition);
    }
}
