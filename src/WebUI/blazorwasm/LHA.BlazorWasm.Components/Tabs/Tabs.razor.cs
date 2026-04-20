using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Tabs;

/// <summary>
/// Enterprise-grade, reusable Tab container for Blazor WebAssembly.
///
/// Supports:
/// - Declarative child TabItem syntax
/// - Dynamic tabs via AddTabAsync / RemoveTabAsync
/// - Two-way bound ActiveIndex / ActiveId
/// - Lazy-load content (load on demand, then preserve DOM)
/// - Closable, disabled, badged tabs
/// - Top / Bottom / Left / Right positions
/// - Line / Pills / Card visual variants
/// - Small / Medium / Large sizes
/// - Scrollable header overflow
/// - Animated ink-bar indicator (Line variant)
/// - Full theme support (dark / light via CSS vars)
///
/// Example:
/// <code>
/// &lt;Tabs @bind-ActiveIndex="activeIndex"&gt;
///     &lt;TabItem Title="Info" Icon="bi bi-info-circle"&gt;
///         &lt;Content&gt;&lt;p&gt;Tab 1 body&lt;/p&gt;&lt;/Content&gt;
///     &lt;/TabItem&gt;
///     &lt;TabItem Title="Settings" Icon="bi bi-gear" Closable="true"&gt;
///         &lt;Content&gt;&lt;p&gt;Tab 2 body&lt;/p&gt;&lt;/Content&gt;
///     &lt;/TabItem&gt;
/// &lt;/Tabs&gt;
/// </code>
/// </summary>
public partial class Tabs : LhaComponentBase
{
    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Layout & Visual
    // ═══════════════════════════════════════════════════════════

    /// <summary>Position of the tab header strip relative to the content panel.</summary>
    [Parameter] public CTabPosition Position { get; set; } = CTabPosition.Top;

    /// <summary>Visual style of the tab header: Line (underline), Pills, or Card.</summary>
    [Parameter] public CTabVariant Variant { get; set; } = CTabVariant.Line;

    /// <summary>Size of the tab header buttons.</summary>
    [Parameter] public CTabSize Size { get; set; } = CTabSize.Medium;

    /// <summary>When true the header strip scrolls horizontally on overflow instead of wrapping.</summary>
    [Parameter] public bool Scrollable { get; set; } = true;

    /// <summary>When true the content fills all remaining vertical space (100%).</summary>
    [Parameter] public bool StretchContent { get; set; }

    /// <summary>Extra CSS classes applied to the root container.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Inline styles applied to the root container.</summary>
    [Parameter] public string? Style { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Active Tab (two-way bindable)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Zero-based index of the currently active tab.
    /// Supports two-way binding: <c>@bind-ActiveIndex="myIndex"</c>
    /// </summary>
    [Parameter] public int ActiveIndex { get; set; }

    /// <summary>Fires when <see cref="ActiveIndex"/> changes.</summary>
    [Parameter] public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>
    /// Unique string ID of the currently active tab.
    /// Supports two-way binding: <c>@bind-ActiveId="myId"</c>
    /// </summary>
    [Parameter] public string? ActiveId { get; set; }

    /// <summary>Fires when <see cref="ActiveId"/> changes.</summary>
    [Parameter] public EventCallback<string?> ActiveIdChanged { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Behaviour
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// When true, tab content is not rendered until the tab is first activated.
    /// Once rendered, the DOM is kept alive (hidden via CSS) to preserve component state.
    /// </summary>
    [Parameter] public bool LazyLoad { get; set; } = true;

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Callbacks
    // ═══════════════════════════════════════════════════════════

    /// <summary>Fires when the active tab changes. Argument is the newly active <see cref="TabDefinition"/>.</summary>
    [Parameter] public EventCallback<TabDefinition> OnTabChanged { get; set; }

    /// <summary>Fires just before a tab is closed. Return false to cancel. Argument is the tab being closed.</summary>
    [Parameter] public EventCallback<TabDefinition> OnTabClose { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Templates / Slots
    // ═══════════════════════════════════════════════════════════

    /// <summary>Declarative child TabItem registrations.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional extra content rendered at the trailing end of the header bar (e.g. an "Add" button).</summary>
    [Parameter] public RenderFragment? HeaderExtra { get; set; }

    /// <summary>Pass-through attributes forwarded to the root div.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // INTERNAL STATE
    // ═══════════════════════════════════════════════════════════

    private readonly List<TabDefinition> _tabs = [];
    private string? _activeId;

    // ═══════════════════════════════════════════════════════════
    // COMPUTED HELPERS
    // ═══════════════════════════════════════════════════════════

    private bool IsVertical => Position is CTabPosition.Left or CTabPosition.Right;

    // ═══════════════════════════════════════════════════════════
    // LIFECYCLE
    // ═══════════════════════════════════════════════════════════

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ThemeState.OnThemeChanged += OnThemeChangedHandler;
    }

    // Wrapper needed because OnThemeChanged is Action<CThemeMode>, not Action.
    private void OnThemeChangedHandler(Services.Theme.CThemeMode _) => InvokeAsync(StateHasChanged);

    protected override void OnParametersSet()
    {
        // Honour external ActiveId binding
        if (!string.IsNullOrWhiteSpace(ActiveId) && ActiveId != _activeId)
        {
            _activeId = ActiveId;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Select first non-disabled tab if no initial selection was bound
            if (string.IsNullOrWhiteSpace(_activeId))
            {
                var ordered = _tabs.OrderBy(t => t.Order).ToList();

                // Honour ActiveIndex parameter
                var byIndex = ordered.ElementAtOrDefault(ActiveIndex);
                var initial = (byIndex is not null && !byIndex.Disabled)
                    ? byIndex
                    : ordered.FirstOrDefault(t => !t.Disabled);

                if (initial is not null)
                {
                    _activeId = initial.Id;
                    initial.HasBeenRendered = true;
                    StateHasChanged();
                }
            }

            await UpdateInkBarAsync();
        }
    }

    // ═══════════════════════════════════════════════════════════
    // PUBLIC API — called by TabItem (registration)
    // ═══════════════════════════════════════════════════════════

    /// <summary>Called by <see cref="TabItem"/> children to self-register.</summary>
    public void RegisterTab(TabDefinition tab)
    {
        tab.Order = _tabs.Count;
        _tabs.Add(tab);
        StateHasChanged();
    }

    /// <summary>Called by <see cref="TabItem.Dispose"/> to self-unregister.</summary>
    public void UnregisterTab(TabDefinition tab)
    {
        _tabs.Remove(tab);

        // If we removed the active tab, fall back to first available
        if (_activeId == tab.Id)
        {
            var next = _tabs.OrderBy(t => t.Order).FirstOrDefault(t => !t.Disabled);
            _activeId = next?.Id;
            if (next is not null) next.HasBeenRendered = true;
        }

        StateHasChanged();
    }

    /// <summary>
    /// Forces a state update on the Tabs container.
    /// Called by <see cref="TabItem.OnParametersSet"/> when a child parameter such as Title or Badge changes.
    /// </summary>
    public void NotifyStateChanged() => InvokeAsync(StateHasChanged);

    // ═══════════════════════════════════════════════════════════
    // PUBLIC API — Dynamic tab management
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Programmatically adds a new tab and optionally activates it.
    /// Returns the ID of the new tab.
    /// </summary>
    public async Task<string> AddTabAsync(
        string title,
        RenderFragment? content = null,
        string? icon = null,
        string? badge = null,
        bool closable = false,
        bool disabled = false,
        bool activate = true)
    {
        var def = new TabDefinition
        {
            Title = title,
            Icon = icon,
            Badge = badge,
            Closable = closable,
            Disabled = disabled,
            Content = content
        };

        _tabs.Add(def);
        def.Order = _tabs.Count - 1;

        if (activate && !disabled)
        {
            await ActivateTabAsync(def);
        }
        else
        {
            StateHasChanged();
        }

        return def.Id;
    }

    /// <summary>
    /// Programmatically removes the tab with the given ID.
    /// Fires <see cref="OnTabClose"/> before removal.
    /// </summary>
    public async Task RemoveTabAsync(string tabId)
    {
        var tab = _tabs.FirstOrDefault(t => t.Id == tabId);
        if (tab is null) return;

        if (OnTabClose.HasDelegate)
            await OnTabClose.InvokeAsync(tab);

        UnregisterTab(tab);
        await UpdateInkBarAsync();
    }

    /// <summary>
    /// Programmatically activates the tab at the given zero-based index.
    /// </summary>
    public async Task SetActiveIndexAsync(int index)
    {
        var tab = _tabs.OrderBy(t => t.Order).ElementAtOrDefault(index);
        if (tab is not null && !tab.Disabled)
            await ActivateTabAsync(tab);
    }

    /// <summary>
    /// Programmatically activates the tab with the given ID.
    /// </summary>
    public async Task SetActiveIdAsync(string id)
    {
        var tab = _tabs.FirstOrDefault(t => t.Id == id);
        if (tab is not null && !tab.Disabled)
            await ActivateTabAsync(tab);
    }

    // ═══════════════════════════════════════════════════════════
    // INTERNAL — Tab activation
    // ═══════════════════════════════════════════════════════════

    private async Task ActivateTabAsync(TabDefinition tab)
    {
        if (tab.Disabled || tab.Id == _activeId) return;

        _activeId = tab.Id;
        tab.HasBeenRendered = true;

        // Compute new active index
        var ordered = _tabs.OrderBy(t => t.Order).ToList();
        var newIndex = ordered.IndexOf(tab);

        // Notify two-way bindings
        if (ActiveIndexChanged.HasDelegate)
            await ActiveIndexChanged.InvokeAsync(newIndex);

        if (ActiveIdChanged.HasDelegate)
            await ActiveIdChanged.InvokeAsync(tab.Id);

        if (OnTabChanged.HasDelegate)
            await OnTabChanged.InvokeAsync(tab);

        StateHasChanged();
        await UpdateInkBarAsync();
    }

    private async Task CloseTabAsync(TabDefinition tab)
    {
        await RemoveTabAsync(tab.Id);
    }

    // ═══════════════════════════════════════════════════════════
    // INTERNAL — Ink bar
    // ═══════════════════════════════════════════════════════════

    private async Task UpdateInkBarAsync()
    {
        if (Variant != CTabVariant.Line) return;

        try
        {
            // Ask JS to measure the active button so we can position the ink bar
            var result = await JS.InvokeAsync<InkBarMeasurement>(
                "lhaTabs.measureActiveTab",
                $"tab-btn-{_activeId}",
                IsVertical);

            _inkStyle = IsVertical
                ? $"top:{result.Offset}px;height:{result.Size}px;"
                : $"left:{result.Offset}px;width:{result.Size}px;";

            StateHasChanged();
        }
        catch
        {
            // JS not available (SSR prerender) — ink bar stays hidden
        }
    }

    // ═══════════════════════════════════════════════════════════
    // INTERNAL — CSS class builders
    // ═══════════════════════════════════════════════════════════

    private string GetRootClass()
    {
        var parts = new List<string> { "tabs-root" };

        parts.Add($"tabs-pos-{Position.ToString().ToLowerInvariant()}");
        parts.Add($"tabs-variant-{Variant.ToString().ToLowerInvariant()}");
        parts.Add($"tabs-size-{Size.ToString().ToLowerInvariant()}");

        parts.Add(ThemeService.ThemeClass);

        if (Scrollable) parts.Add("tabs-scrollable");
        if (StretchContent) parts.Add("tabs-stretch");
        if (!string.IsNullOrWhiteSpace(Class)) parts.Add(Class);

        return string.Join(" ", parts);
    }

    private string GetTabHeaderClass(TabDefinition tab, bool isActive)
    {
        var parts = new List<string> { "tab-item" };

        if (isActive) parts.Add("tab-item-active");
        if (tab.Disabled) parts.Add("tab-item-disabled");
        if (tab.Closable) parts.Add("tab-item-closable");
        if (!string.IsNullOrWhiteSpace(tab.HeaderClass)) parts.Add(tab.HeaderClass);

        return string.Join(" ", parts);
    }

    private string GetPanelClass(TabDefinition tab, bool isActive)
    {
        var parts = new List<string> { "tab-panel" };

        if (isActive) parts.Add("tab-panel-active");
        if (!string.IsNullOrWhiteSpace(tab.ContentClass)) parts.Add(tab.ContentClass);

        return string.Join(" ", parts);
    }

    // ═══════════════════════════════════════════════════════════
    // DISPOSE
    // ═══════════════════════════════════════════════════════════

    public override void Dispose()
    {
        ThemeState.OnThemeChanged -= OnThemeChangedHandler;
        base.Dispose();
    }

    // ── JS result model ──
    private sealed record InkBarMeasurement(double Offset, double Size);
}
