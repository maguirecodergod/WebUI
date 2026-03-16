using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Services.Storage;
using LHA.BlazorWasm.Services.Theme;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Sidebar;

/// <summary>
/// Enterprise-grade sidebar navigation component with recursive tree rendering,
/// resizable width, responsive breakpoints, and theme/localization integration.
/// </summary>
public partial class Sidebar : LhaComponentBase, IAsyncDisposable
{
    #region ── Injected Services ──

    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private IThemeService ThemeService { get; set; } = default!;

    #endregion

    #region ── Parameters ──

    /// <summary>
    /// The hierarchical collection of sidebar navigation items.
    /// Supports N-level nesting via <see cref="SidebarItemModel.Children"/>.
    /// </summary>
    [Parameter] public List<SidebarItemModel>? Items { get; set; }

    /// <summary>
    /// The current visual state of the sidebar. Bind two-way with <see cref="StateChanged"/>.
    /// </summary>
    [Parameter] public SidebarState State { get; set; } = SidebarState.Expanded;

    /// <summary>
    /// Two-way binding callback for <see cref="State"/>.
    /// </summary>
    [Parameter] public EventCallback<SidebarState> StateChanged { get; set; }

    /// <summary>
    /// Whether the sidebar edge can be dragged to resize width. Desktop only.
    /// </summary>
    [Parameter] public bool Resizable { get; set; } = true;

    /// <summary>
    /// Minimum allowed width in pixels when resizing. Default is 200px.
    /// </summary>
    [Parameter] public int MinWidth { get; set; } = 200;

    /// <summary>
    /// Maximum allowed width in pixels when resizing. Default is 480px.
    /// </summary>
    [Parameter] public int MaxWidth { get; set; } = 480;

    /// <summary>
    /// Default width in pixels for the expanded sidebar. Default is 280px.
    /// </summary>
    [Parameter] public int DefaultWidth { get; set; } = 280;

    /// <summary>
    /// Width in pixels for the mini/collapsed sidebar. Default is 64px.
    /// </summary>
    [Parameter] public int MiniWidth { get; set; } = 64;

    /// <summary>
    /// Whether the mini sidebar should expand on hover. Default is true.
    /// </summary>
    [Parameter] public bool ExpandOnHover { get; set; } = true;

    /// <summary>
    /// Optional header template rendered at the top of the sidebar (e.g., branding/logo).
    /// </summary>
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Optional footer template rendered at the bottom of the sidebar (e.g., user profile).
    /// </summary>
    [Parameter] public RenderFragment? FooterTemplate { get; set; }

    /// <summary>
    /// Callback invoked when a leaf item is clicked, passing the clicked <see cref="SidebarItemModel"/>.
    /// </summary>
    [Parameter] public EventCallback<SidebarItemModel> OnItemClick { get; set; }

    /// <summary>
    /// Callback invoked when the sidebar width changes (after resize or state change).
    /// </summary>
    [Parameter] public EventCallback<int> OnWidthChanged { get; set; }

    /// <summary>
    /// Additional CSS class(es) to apply to the root sidebar container.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Additional inline styles for the root sidebar container.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// The breakpoint width (in px) at which the sidebar switches to Mini mode.
    /// Default is 1024.
    /// </summary>
    [Parameter] public int TabletBreakpoint { get; set; } = 1024;

    /// <summary>
    /// The breakpoint width (in px) at which the sidebar switches to Hidden/Drawer mode.
    /// Default is 768.
    /// </summary>
    [Parameter] public int MobileBreakpoint { get; set; } = 768;

    #endregion

    #region ── Private State ──

    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<Sidebar>? _dotNetRef;
    private ElementReference _sidebarRef;

    private HashSet<string> _expandedIds = new();
    private string? _activeItemId;
    private int _currentWidth;
    private bool _isDragging;
    private bool _mobileOpen;
    private bool _hoverExpanded;
    private bool _isDisposed;

    // Snapshot for ShouldRender optimization
    private string? _previousItemsJson;
    private SidebarState _previousState;
    private int _previousWidth;

    private const string WidthStorageKey = "lha:sidebar:width";
    private const string ExpandedIdsStorageKey = "lha:sidebar:expanded";

    #endregion

    #region ── Computed Properties ──

    /// <summary>
    /// The effective display state: if hover-expanding a mini sidebar, treat as Expanded.
    /// </summary>
    private SidebarState EffectiveState =>
        State == SidebarState.Mini && _hoverExpanded ? SidebarState.Expanded : State;

    private string StateClass => State switch
    {
        SidebarState.Expanded => "lha-sidebar--expanded",
        SidebarState.Mini => "lha-sidebar--mini",
        SidebarState.Hidden => _mobileOpen ? "lha-sidebar--drawer-open" : "lha-sidebar--hidden",
        _ => "lha-sidebar--expanded"
    };

    private string HoverExpandClass =>
        _hoverExpanded && State == SidebarState.Mini ? "lha-sidebar--hover-expanded" : "";

    private string SidebarStyle
    {
        get
        {
            var width = EffectiveState switch
            {
                SidebarState.Expanded => _currentWidth > 0 ? _currentWidth : DefaultWidth,
                SidebarState.Mini => MiniWidth,
                SidebarState.Hidden => 0,
                _ => DefaultWidth
            };

            var styles = new List<string>
            {
                $"--sidebar-width: {width}px",
                $"--sidebar-mini-width: {MiniWidth}px",
                $"--sidebar-default-width: {DefaultWidth}px"
            };

            if (!string.IsNullOrEmpty(Style))
                styles.Add(Style);

            return string.Join("; ", styles);
        }
    }

    #endregion

    #region ── Lifecycle ──

    protected override async Task OnInitializedAsync()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _currentWidth = DefaultWidth;

        // Restore persisted width
        var savedWidth = await LocalStorage.GetAsync<int?>(WidthStorageKey);
        if (savedWidth.HasValue && savedWidth.Value >= MinWidth && savedWidth.Value <= MaxWidth)
        {
            _currentWidth = savedWidth.Value;
        }

        // Restore persisted expanded nodes
        var savedIds = await LocalStorage.GetAsync<List<string>?>(ExpandedIdsStorageKey);
        if (savedIds != null)
        {
            _expandedIds = new HashSet<string>(savedIds);
        }

        // Detect active item from current route
        DetectActiveItem();

        // Subscribe to navigation changes
        Navigation.LocationChanged += OnLocationChanged;

        // Subscribe to localization changes
        Localizer.OnLanguageChanged += OnLanguageChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/LHA.BlazorWasm.Components/Sidebar/Sidebar.razor.js");

            await _jsModule.InvokeVoidAsync("initialize", _sidebarRef, _dotNetRef,
                TabletBreakpoint, MobileBreakpoint);
        }
    }

    protected override bool ShouldRender()
    {
        // Quick state checks for trivial changes
        if (_previousState != State || _previousWidth != _currentWidth)
        {
            _previousState = State;
            _previousWidth = _currentWidth;
            return true;
        }

        // Deep comparison of tree state via serialization
        var currentJson = System.Text.Json.JsonSerializer.Serialize(
            new { Items, ExpandedIds = _expandedIds, _activeItemId, _mobileOpen, _hoverExpanded });

        if (_previousItemsJson != currentJson)
        {
            _previousItemsJson = currentJson;
            return true;
        }

        return false;
    }

    #endregion

    #region ── Public API ──

    /// <summary>
    /// Sets the sidebar state programmatically.
    /// </summary>
    public async Task SetStateAsync(SidebarState newState)
    {
        if (State != newState)
        {
            State = newState;
            await StateChanged.InvokeAsync(State);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Toggles the mobile off-canvas drawer open/closed.
    /// </summary>
    public void ToggleMobileDrawer()
    {
        _mobileOpen = !_mobileOpen;
        StateHasChanged();
    }

    /// <summary>
    /// Expands all items in the tree recursively.
    /// </summary>
    public async Task ExpandAllAsync()
    {
        if (Items == null) return;
        CollectAllIds(Items, _expandedIds);
        await PersistExpandedIdsAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Collapses all items in the tree.
    /// </summary>
    public async Task CollapseAllAsync()
    {
        _expandedIds.Clear();
        await PersistExpandedIdsAsync();
        StateHasChanged();
    }

    #endregion

    #region ── JS Interop Callbacks ──

    /// <summary>
    /// Called from JS when viewport crosses a responsive breakpoint.
    /// </summary>
    [JSInvokable]
    public async Task OnBreakpointChanged(string breakpoint)
    {
        var newState = breakpoint switch
        {
            "mobile" => SidebarState.Hidden,
            "tablet" => SidebarState.Mini,
            _ => SidebarState.Expanded
        };

        if (State != newState)
        {
            State = newState;
            _mobileOpen = false;
            _hoverExpanded = false;
            await StateChanged.InvokeAsync(State);
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Called from JS during a resize drag to update the sidebar width smoothly.
    /// The JS handles mousemove on the document; this callback updates the Blazor state.
    /// </summary>
    [JSInvokable]
    public async Task OnResizeUpdate(int newWidth)
    {
        var clampedWidth = Math.Clamp(newWidth, MinWidth, MaxWidth);
        if (_currentWidth != clampedWidth)
        {
            _currentWidth = clampedWidth;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Called from JS when the resize drag ends.
    /// </summary>
    [JSInvokable]
    public async Task OnResizeEnd(int finalWidth)
    {
        _isDragging = false;
        _currentWidth = Math.Clamp(finalWidth, MinWidth, MaxWidth);
        await LocalStorage.SetAsync(WidthStorageKey, _currentWidth);
        await OnWidthChanged.InvokeAsync(_currentWidth);
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region ── Event Handlers ──

    private async Task StartResize()
    {
        if (_jsModule == null || !Resizable || EffectiveState != SidebarState.Expanded) return;

        _isDragging = true;
        await _jsModule.InvokeVoidAsync("startResize", _sidebarRef, _dotNetRef, MinWidth, MaxWidth);
    }

    private void HandleMouseEnter()
    {
        if (State == SidebarState.Mini && ExpandOnHover)
        {
            _hoverExpanded = true;
        }
    }

    private void HandleMouseLeave()
    {
        if (_hoverExpanded)
        {
            _hoverExpanded = false;
        }
    }

    private async Task HandleToggleExpand(string itemId)
    {
        if (!_expandedIds.Remove(itemId))
        {
            _expandedIds.Add(itemId);
        }

        await PersistExpandedIdsAsync();
    }

    private async Task HandleItemClick(SidebarItemModel item)
    {
        _activeItemId = item.Id;

        // Auto-close mobile drawer on navigation
        if (_mobileOpen)
        {
            _mobileOpen = false;
        }

        await OnItemClick.InvokeAsync(item);
    }

    private void CloseMobileDrawer()
    {
        _mobileOpen = false;
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        DetectActiveItem();
        InvokeAsync(StateHasChanged);
    }

    private void OnLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    #endregion

    #region ── Helpers ──

    private void DetectActiveItem()
    {
        if (Items == null) return;

        var currentUri = Navigation.ToBaseRelativePath(Navigation.Uri);
        if (!currentUri.StartsWith("/"))
            currentUri = "/" + currentUri;

        var (activeItem, parentChain) = FindActiveItem(Items, currentUri);

        if (activeItem != null)
        {
            _activeItemId = activeItem.Id;

            // Auto-expand parent nodes leading to the active item
            foreach (var parentId in parentChain)
            {
                _expandedIds.Add(parentId);
            }
        }
    }

    private (SidebarItemModel? item, List<string> parentChain) FindActiveItem(
        List<SidebarItemModel> items, string currentUri, List<string>? ancestors = null)
    {
        ancestors ??= new List<string>();

        foreach (var item in items.Where(i => i.IsVisible && !i.IsDivider))
        {
            if (!string.IsNullOrEmpty(item.Href))
            {
                var href = item.Href.TrimEnd('/');
                var uri = currentUri.TrimEnd('/');

                var isMatch = item.MatchMode == NavLinkMatchMode.Exact
                    ? string.Equals(uri, href, StringComparison.OrdinalIgnoreCase)
                    : uri.StartsWith(href, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                {
                    return (item, ancestors.ToList());
                }
            }

            if (item.HasChildren)
            {
                var childAncestors = new List<string>(ancestors) { item.Id };
                var result = FindActiveItem(item.Children, currentUri, childAncestors);
                if (result.item != null)
                    return result;
            }
        }

        return (null, new List<string>());
    }

    private static void CollectAllIds(List<SidebarItemModel> items, HashSet<string> ids)
    {
        foreach (var item in items)
        {
            if (item.HasChildren)
            {
                ids.Add(item.Id);
                CollectAllIds(item.Children, ids);
            }
        }
    }

    private async Task PersistExpandedIdsAsync()
    {
        await LocalStorage.SetAsync(ExpandedIdsStorageKey, _expandedIds.ToList());
    }

    #endregion

    #region ── Disposal ──

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        Navigation.LocationChanged -= OnLocationChanged;
        Localizer.OnLanguageChanged -= OnLanguageChanged;

        if (_jsModule != null)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("dispose");
                await _jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore during app shutdown
            }
        }

        _dotNetRef?.Dispose();
    }

    #endregion
}
