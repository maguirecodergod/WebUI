using LHA.BlazorWasm.Services.Storage;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Form;

namespace LHA.BlazorWasm.Components.Table;

/// <summary>
/// Enterprise DataTable — reusable, production-level generic data table component.
/// Supports client-side &amp; server-side modes, sorting, filtering, paging,
/// selection, column management, expandable rows, virtualization, skeleton loading.
/// </summary>
public partial class DataTable<TItem> : LhaComponentBase, IDisposable
{
    // ═══════════════════════════════════════════════════════════
    // INJECTED SERVICES
    // ═══════════════════════════════════════════════════════════

    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    private string ThemeClass => ThemeService.ThemeClass;

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Data
    // ═══════════════════════════════════════════════════════════

    /// <summary>Full dataset for client-side mode. Also used in manual server-side mode.</summary>
    [Parameter] public IReadOnlyList<TItem>? Items { get; set; }

    /// <summary>Async data provider for server-side mode. Called on every state change.</summary>
    [Parameter] public Func<DataTableRequest, Task<DataTableResponse<TItem>>>? ServerDataSource { get; set; }

    /// <summary>Manual server-side callback. Parent manages data externally.</summary>
    [Parameter] public EventCallback<DataTableRequest> OnDataRequest { get; set; }

    /// <summary>Total record count override (manual server-side mode).</summary>
    [Parameter] public int? TotalCount { get; set; }

    /// <summary>Data source mode.</summary>
    [Parameter] public DataTableMode Mode { get; set; } = DataTableMode.ClientSide;

    /// <summary>Stable key selector for efficient diffing and cross-page selection tracking.</summary>
    [Parameter] public Func<TItem, object>? KeySelector { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Features
    // ═══════════════════════════════════════════════════════════

    [Parameter] public bool Searchable { get; set; }
    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public bool ShowPager { get; set; } = true;
    [Parameter] public bool ShowInfo { get; set; } = true;
    [Parameter] public bool ShowColumnToggle { get; set; } = true;
    [Parameter] public SelectionMode SelectionMode { get; set; } = SelectionMode.None;
    [Parameter] public bool SelectOnRowClick { get; set; }
    [Parameter] public bool AllowMultiSort { get; set; }
    [Parameter] public bool Striped { get; set; } = true;
    [Parameter] public bool Hoverable { get; set; } = true;
    [Parameter] public bool Bordered { get; set; }
    [Parameter] public bool Compact { get; set; }
    [Parameter] public bool VirtualizeRows { get; set; }
    [Parameter] public float VirtualItemSize { get; set; } = 48f;
    [Parameter] public string VirtualHeight { get; set; } = "600px";
    [Parameter] public int[] PageSizes { get; set; } = [10, 25, 50, 100];
    [Parameter] public int DefaultPageSize { get; set; } = 10;
    [Parameter] public int SearchDebounceMs { get; set; } = 350;

    /// <summary>Max height for the table body area. Enables vertical scrolling with sticky header.</summary>
    [Parameter] public string? MaxHeight { get; set; } = "700px";

    /// <summary>Unique table identifier for persisting user settings (column visibility, etc.).</summary>
    [Parameter] public string? TableId { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Templates
    // ═══════════════════════════════════════════════════════════

    /// <summary>Column definitions (DataTableColumn children).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Custom empty state.</summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>Expandable row detail template.</summary>
    [Parameter] public RenderFragment<TItem>? ExpandedRowTemplate { get; set; }

    /// <summary>Extra toolbar content (e.g. export button).</summary>
    [Parameter] public RenderFragment? ToolbarTemplate { get; set; }

    /// <summary>Custom loading skeleton override.</summary>
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Callbacks
    // ═══════════════════════════════════════════════════════════

    [Parameter] public EventCallback<IReadOnlyList<TItem>> OnSelectionChanged { get; set; }
    [Parameter] public EventCallback<TItem> OnRowClick { get; set; }
    [Parameter] public EventCallback<TItem> OnRowDoubleClick { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PARAMETERS — Styling
    // ═══════════════════════════════════════════════════════════

    [Parameter] public string? CssClass { get; set; }
    [Parameter] public Func<TItem, string?>? RowClass { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Programmatic column definitions (alternative to DataTableColumn children).</summary>
    [Parameter] public List<ColumnDefinition<TItem>>? Columns { get; set; }

    /// <summary>Default sort definitions applied on first load.</summary>
    [Parameter] public IReadOnlyList<SortDefinition>? DefaultSorts { get; set; }

    // ═══════════════════════════════════════════════════════════
    // INTERNAL STATE
    // ═══════════════════════════════════════════════════════════

    private readonly List<ColumnDefinition<TItem>> _columns = [];
    private List<ColumnDefinition<TItem>> VisibleColumns =>
        _columns.Where(c => c.Visible).OrderBy(c => c.Order).ToList();

    private readonly DataTableRequest _request = new();
    private IReadOnlyList<TItem> _displayItems = [];
    private IReadOnlyList<TItem> _allProcessedItems = []; // for virtualize mode
    private int _totalCount;
    private bool _isLoading;
    private bool _isFirstLoad = true;
    private IReadOnlyList<TItem>? _previousItems;

    // Selection
    private readonly Dictionary<object, TItem> _selectedItems = new();
    private bool _allPageSelected;
    private bool _allEntireDatasetSelected;
    private HashSet<string>? _restoredSelectedKeys;

    // Expanded rows
    private readonly HashSet<object> _expandedItems = new();

    // UI state
    private bool _columnToggleOpen;
    private bool _filterSidebarOpen;
    private string? _searchInput;
    private string? _dummyFilter { get; set; } // Required for dynamically generated CInput ValueExpression

    // Active filter count for badge
    private int ActiveFilterCount => _request.Filters.Count(f => f.IsActive);

    // Debouncers
    private Debouncer? _searchDebouncer;
    private Debouncer? _filterDebouncer;

    // Filter Staging
    private List<FilterDefinition> _stagedFilters = new();

    // ── Computed ──
    private int _totalColumns
    {
        get
        {
            var cols = VisibleColumns.Count;
            if (SelectionMode != SelectionMode.None) cols++;
            if (ExpandedRowTemplate is not null) cols++;
            return cols;
        }
    }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)_totalCount / _request.PageSize));

    private string TableCss
    {
        get
        {
            var parts = new List<string>(6) { "dt-table" };
            if (Striped) parts.Add("dt-striped");
            if (Hoverable) parts.Add("dt-hoverable");
            if (Bordered) parts.Add("dt-bordered");
            if (Compact) parts.Add("dt-compact");
            return string.Join(' ', parts);
        }
    }

    // ═══════════════════════════════════════════════════════════
    // COLUMN MANAGEMENT (called by DataTableColumn children)
    // ═══════════════════════════════════════════════════════════

    /// <summary>Register a column definition (called by DataTableColumn.OnInitialized).</summary>
    public void AddColumn(ColumnDefinition<TItem> column)
    {
        if (_columns.All(c => c.Id != column.Id))
        {
            column.Order = _columns.Count;
            _columns.Add(column);
        }
    }

    /// <summary>Remove a column definition (called by DataTableColumn.Dispose).</summary>
    public void RemoveColumn(ColumnDefinition<TItem> column)
    {
        _columns.RemoveAll(c => c.Id == column.Id);
    }

    // ═══════════════════════════════════════════════════════════
    // LIFECYCLE
    // ═══════════════════════════════════════════════════════════

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ThemeState.OnThemeChanged += OnThemeChanged;
        _request.PageSize = DefaultPageSize;
        _searchDebouncer = new Debouncer(SearchDebounceMs);
        _filterDebouncer = new Debouncer(SearchDebounceMs);

        // Merge programmatic columns
        if (Columns is { Count: > 0 })
        {
            foreach (var col in Columns)
            {
                col.Order = _columns.Count;
                _columns.Add(col);
            }
        }

        // Apply default sorts
        if (DefaultSorts is { Count: > 0 })
        {
            _request.Sorts.AddRange(DefaultSorts);
        }
    }

    protected override void OnParametersSet()
    {
        if (Mode == DataTableMode.ClientSide && Items is not null && !ReferenceEquals(Items, _previousItems))
        {
            _previousItems = Items;
            ProcessClientData();
        }
        else if (Mode == DataTableMode.ServerSide && ServerDataSource is null)
        {
            // Manual server mode: parent provides Items + TotalCount
            _displayItems = Items ?? [];
            _totalCount = TotalCount ?? _displayItems.Count;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _isFirstLoad)
        {
            _isFirstLoad = false;

            // Restore persisted column visibility & selection
            await RestoreColumnVisibilityAsync();
            await RestoreSelectionAsync();

            if (Mode == DataTableMode.ServerSide && ServerDataSource is not null)
            {
                await LoadServerDataAsync();
            }
            else
            {
                SyncSelectedItems();
                StateHasChanged();
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    // DATA LOADING
    // ═══════════════════════════════════════════════════════════

    /// <summary>Public API: Refresh data from source.</summary>
    public async Task RefreshAsync()
    {
        if (Mode == DataTableMode.ServerSide && ServerDataSource is not null)
        {
            await LoadServerDataAsync();
        }
        else if (Mode == DataTableMode.ServerSide && OnDataRequest.HasDelegate)
        {
            _isLoading = true;
            StateHasChanged();
            await OnDataRequest.InvokeAsync(_request);
            _isLoading = false;
            StateHasChanged();
        }
        else
        {
            ProcessClientData();
            StateHasChanged();
        }
    }

    private async Task LoadServerDataAsync()
    {
        if (ServerDataSource is null) return;

        _isLoading = true;
        StateHasChanged();

        try
        {
            var response = await ServerDataSource(_request);
            _displayItems = response.Items;
            _totalCount = response.TotalCount;
            SyncSelectedItems();
        }
        catch
        {
            _displayItems = [];
            _totalCount = 0;
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    // ═══════════════════════════════════════════════════════════
    // CLIENT-SIDE PROCESSING
    // ═══════════════════════════════════════════════════════════

    private void ProcessClientData()
    {
        if (Items is null)
        {
            _displayItems = [];
            _allProcessedItems = [];
            _totalCount = 0;
            return;
        }

        var query = Items.AsEnumerable();

        // ── Global search ──
        if (!string.IsNullOrWhiteSpace(_request.SearchTerm))
        {
            var search = _request.SearchTerm;
            var visibleCols = VisibleColumns;
            query = query.Where(item => visibleCols.Any(col =>
                col.GetDisplayValue(item).Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        // ── Column filters ──
        foreach (var filter in _request.Filters.Where(f => f.IsActive))
        {
            var col = _columns.FirstOrDefault(c => c.Field == filter.Field);
            if (col?.ValueSelector is null) continue;
            query = ApplyClientFilter(query, col, filter);
        }

        // ── Sort ──
        IOrderedEnumerable<TItem>? ordered = null;
        foreach (var sort in _request.Sorts.Where(s => s.Direction != SortDirection.None))
        {
            var col = _columns.FirstOrDefault(c => c.Field == sort.Field);
            if (col?.ValueSelector is null) continue;

            if (ordered is null)
                ordered = sort.Direction == SortDirection.Ascending
                    ? query.OrderBy(col.ValueSelector)
                    : query.OrderByDescending(col.ValueSelector);
            else
                ordered = sort.Direction == SortDirection.Ascending
                    ? ordered.ThenBy(col.ValueSelector)
                    : ordered.ThenByDescending(col.ValueSelector);
        }
        if (ordered is not null) query = ordered;

        var allItems = query.ToList();
        _totalCount = allItems.Count;
        _allProcessedItems = allItems;

        // ── Page (skip when virtualizing) ──
        if (VirtualizeRows)
        {
            _displayItems = allItems;
        }
        else
        {
            // Clamp page index
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalCount / _request.PageSize));
            if (_request.PageNumber > totalPages)
                _request.PageNumber = totalPages;

            _displayItems = allItems
                .Skip((_request.PageNumber - 1) * _request.PageSize)
                .Take(_request.PageSize)
                .ToList();
        }

        SyncSelectedItems();
        UpdatePageSelectionState();
    }

    private static IEnumerable<TItem> ApplyClientFilter(
        IEnumerable<TItem> query, ColumnDefinition<TItem> col, FilterDefinition filter)
    {
        return filter.Operator switch
        {
            FilterOperator.Contains => query.Where(item =>
                col.GetDisplayValue(item).Contains(filter.Value!, StringComparison.OrdinalIgnoreCase)),

            FilterOperator.Equals => query.Where(item =>
                col.GetDisplayValue(item).Equals(filter.Value!, StringComparison.OrdinalIgnoreCase)),

            FilterOperator.NotEquals => query.Where(item =>
                !col.GetDisplayValue(item).Equals(filter.Value!, StringComparison.OrdinalIgnoreCase)),

            FilterOperator.StartsWith => query.Where(item =>
                col.GetDisplayValue(item).StartsWith(filter.Value!, StringComparison.OrdinalIgnoreCase)),

            FilterOperator.EndsWith => query.Where(item =>
                col.GetDisplayValue(item).EndsWith(filter.Value!, StringComparison.OrdinalIgnoreCase)),

            FilterOperator.GreaterThan => query.Where(item =>
                CompareValues(col.ValueSelector!(item), filter.Value) > 0),

            FilterOperator.GreaterThanOrEqual => query.Where(item =>
                CompareValues(col.ValueSelector!(item), filter.Value) >= 0),

            FilterOperator.LessThan => query.Where(item =>
                CompareValues(col.ValueSelector!(item), filter.Value) < 0),

            FilterOperator.LessThanOrEqual => query.Where(item =>
                CompareValues(col.ValueSelector!(item), filter.Value) <= 0),

            FilterOperator.Between => query.Where(item =>
                CompareValues(col.ValueSelector!(item), filter.Value) >= 0 &&
                CompareValues(col.ValueSelector!(item), filter.ValueTo) <= 0),

            _ => query
        };
    }

    private static int CompareValues(object? a, string? b)
    {
        if (a is null || b is null) return 0;
        try
        {
            if (a is IComparable comparable)
            {
                var converted = Convert.ChangeType(b, a.GetType());
                return comparable.CompareTo(converted);
            }
        }
        catch { /* fall through to string compare */ }
        return string.Compare(a.ToString(), b, StringComparison.Ordinal);
    }

    // ═══════════════════════════════════════════════════════════
    // SORTING
    // ═══════════════════════════════════════════════════════════

    private async Task ToggleSort(string field)
    {
        var existing = _request.Sorts.FirstOrDefault(s => s.Field == field);

        if (!AllowMultiSort)
            _request.Sorts.Clear();

        if (existing is not null)
        {
            _request.Sorts.Remove(existing);
            var toggled = existing.Toggle();
            if (toggled.Direction != SortDirection.None)
                _request.Sorts.Add(toggled);
        }
        else
        {
            _request.Sorts.Add(new SortDefinition(field, SortDirection.Ascending));
        }

        _request.PageNumber = 1;
        await RefreshAsync();
    }

    private SortDefinition GetSort(string field) =>
        _request.Sorts.FirstOrDefault(s => s.Field == field)
        ?? new SortDefinition(field, SortDirection.None);

    // ═══════════════════════════════════════════════════════════
    // FILTERING
    // ═══════════════════════════════════════════════════════════

    private void SetFilter(string field, string? value, FilterType type,
        FilterOperator op = FilterOperator.Contains, string? valueTo = null)
    {
        var existing = _request.Filters.FirstOrDefault(f => f.Field == field);
        if (existing is not null) _request.Filters.Remove(existing);

        if (!string.IsNullOrEmpty(value))
        {
            _request.Filters.Add(new FilterDefinition
            {
                Field = field,
                Type = type,
                Operator = op,
                Value = value,
                ValueTo = valueTo
            });
        }

        _request.PageNumber = 1;
    }

    private async Task HandleFilterTextInput(string field, string? value, FilterType type)
    {
        SetFilter(field, value, type);
        if (_filterDebouncer is not null)
        {
            await _filterDebouncer.DebounceAsync(async () => await RefreshAsync());
        }
        else
        {
            await RefreshAsync();
        }
    }

    private async Task HandleFilterChange(string field, string? value, FilterType type, FilterOperator op)
    {
        SetFilter(field, value, type, op);
        await RefreshAsync();
    }

    private async Task HandleFilterRangeChange(string field, string? from, string? to, FilterType type)
    {
        SetFilter(field, from, type, FilterOperator.Between, to);
        await RefreshAsync();
    }

    private string? GetFilterValue(string field) =>
        _request.Filters.FirstOrDefault(f => f.Field == field)?.Value;

    private string? GetFilterValueTo(string field) =>
        _request.Filters.FirstOrDefault(f => f.Field == field)?.ValueTo;

    // ═══════════════════════════════════════════════════════════
    // SEARCH (global, debounced)
    // ═══════════════════════════════════════════════════════════

    private async Task OnSearchValueChanged(string? value)
    {
        _searchInput = value;

        if (_searchDebouncer is not null)
        {
            await _searchDebouncer.DebounceAsync(async () =>
            {
                _request.SearchTerm = _searchInput;
                _request.PageNumber = 1;
                await RefreshAsync();
            });
        }
    }

    // ═══════════════════════════════════════════════════════════
    // PAGING
    // ═══════════════════════════════════════════════════════════

    private async Task HandlePageChanged(int page)
    {
        _request.PageNumber = Math.Max(1, Math.Min(page, TotalPages));
        await RefreshAsync();
    }

    private async Task HandlePageSizeChanged(int size)
    {
        _request.PageSize = size;
        _request.PageNumber = 1;
        await RefreshAsync();
    }

    // ═══════════════════════════════════════════════════════════
    // SELECTION
    // ═══════════════════════════════════════════════════════════

    private object GetItemKey(TItem item) => KeySelector?.Invoke(item) ?? (object)item!;

    private bool IsSelected(TItem item)
    {
        if (_allEntireDatasetSelected) return true;
        var key = GetItemKey(item);
        if (_selectedItems.ContainsKey(key)) return true;
        if (_restoredSelectedKeys != null && _restoredSelectedKeys.Contains(key?.ToString() ?? "")) return true;
        return false;
    }

    private async Task HandleSelectionToggle(TItem item, object? checkedValue)
    {
        var key = GetItemKey(item);
        var isChecked = checkedValue is true or "true" or "True";

        if (SelectionMode == SelectionMode.Single)
        {
            _selectedItems.Clear();
            if (isChecked)
                _selectedItems[key] = item;
        }
        else
        {
            if (isChecked)
                _selectedItems[key] = item;
            else
                _selectedItems.Remove(key);
        }

        _allEntireDatasetSelected = false;
        UpdatePageSelectionState();
        await PersistSelectionAsync();
        await NotifySelectionChanged();
    }

    private async Task ToggleSelectAllPage()
    {
        if (_allPageSelected)
        {
            // Deselect all on current page
            foreach (var item in _displayItems)
                _selectedItems.Remove(GetItemKey(item));
            _allPageSelected = false;
        }
        else
        {
            // Select all on current page
            foreach (var item in _displayItems)
                _selectedItems[GetItemKey(item)] = item;
            _allPageSelected = true;
        }

        _allEntireDatasetSelected = false;
        await PersistSelectionAsync();
        await NotifySelectionChanged();
    }

    private async Task SelectAllEntireDataset()
    {
        if (Mode == DataTableMode.ClientSide && _allProcessedItems.Count > 0)
        {
            foreach (var item in _allProcessedItems)
                _selectedItems[GetItemKey(item)] = item;
        }
        _allEntireDatasetSelected = true;
        _allPageSelected = true;
        await PersistSelectionAsync();
        await NotifySelectionChanged();
    }

    private async Task ClearSelection()
    {
        _selectedItems.Clear();
        _restoredSelectedKeys?.Clear();
        _allPageSelected = false;
        _allEntireDatasetSelected = false;
        await PersistSelectionAsync();
        await NotifySelectionChanged();
    }

    private void UpdatePageSelectionState()
    {
        _allPageSelected = _displayItems.Count > 0 &&
            _displayItems.All(i => IsSelected(i));
    }

    private async Task NotifySelectionChanged()
    {
        if (OnSelectionChanged.HasDelegate)
        {
            // Note: Notify only items we have actual objects for
            var items = _selectedItems.Values.Where(v => v is not null).ToList();
            await OnSelectionChanged.InvokeAsync(items);
        }
    }

    private async Task HandleRowClickInternal(TItem item)
    {
        if (SelectOnRowClick && SelectionMode != SelectionMode.None)
        {
            var isCurrentlySelected = IsSelected(item);
            await HandleSelectionToggle(item, !isCurrentlySelected);
        }

        if (OnRowClick.HasDelegate)
            await OnRowClick.InvokeAsync(item);
    }

    // ═══════════════════════════════════════════════════════════
    // EXPANDED ROWS
    // ═══════════════════════════════════════════════════════════

    private bool IsExpanded(TItem item) => _expandedItems.Contains(GetItemKey(item));

    private void ToggleExpand(TItem item)
    {
        var key = GetItemKey(item);
        if (!_expandedItems.Remove(key))
            _expandedItems.Add(key);
    }

    // ═══════════════════════════════════════════════════════════
    // COLUMN TOGGLE
    // ═══════════════════════════════════════════════════════════

    private void ToggleColumnDropdown() => _columnToggleOpen = !_columnToggleOpen;

    private void CloseColumnDropdown() => _columnToggleOpen = false;

    private async Task ToggleColumnVisibility(ColumnDefinition<TItem> col, object? checkedValue)
    {
        var wantsVisible = checkedValue is true or "true" or "True";

        // Prevent hiding the last visible column
        if (!wantsVisible && _columns.Count(c => c.Visible) <= 1)
            return;

        col.Visible = wantsVisible;
        col.VisibilityOverridden = true;
        await PersistColumnVisibilityAsync();
    }

    // ── Persistence helpers ──
    private string ColumnStorageKey => $"dt-col-vis-{TableId}";

    /// <summary>Unique storage identifier for a column (handles columns without Field, e.g. "Actions").</summary>
    private static string GetColumnPersistKey(ColumnDefinition<TItem> col)
        => !string.IsNullOrEmpty(col.Field) ? col.Field : $"_title_{col.Title}";

    private async Task PersistColumnVisibilityAsync()
    {
        if (string.IsNullOrEmpty(TableId)) return;
        try
        {
            var state = new Dictionary<string, bool>();
            foreach (var col in _columns)
                state[GetColumnPersistKey(col)] = col.Visible;
            await LocalStorage.SetAsync(ColumnStorageKey, state);
        }
        catch { /* localStorage unavailable during prerender */ }
    }

    private async Task RestoreColumnVisibilityAsync()
    {
        if (string.IsNullOrEmpty(TableId)) return;
        try
        {
            var state = await LocalStorage.GetAsync<Dictionary<string, bool>>(ColumnStorageKey);
            if (state is null) return;
            foreach (var col in _columns)
            {
                if (state.TryGetValue(GetColumnPersistKey(col), out var visible))
                {
                    col.Visible = visible;
                    col.VisibilityOverridden = true;
                }
            }
        }
        catch { /* ignore corrupted data */ }
    }

    private string SelectionStorageKey => $"dt-sel-{TableId}";

    private async Task PersistSelectionAsync()
    {
        if (string.IsNullOrEmpty(TableId) || SelectionMode == SelectionMode.None) return;
        try
        {
            // Combine current active selection keys with any still-restored keys
            var allKeys = _selectedItems.Keys
                .Select(k => k?.ToString() ?? "")
                .ToList();

            if (_restoredSelectedKeys != null)
            {
                foreach (var rk in _restoredSelectedKeys)
                {
                    if (!allKeys.Contains(rk)) allKeys.Add(rk);
                }
            }

            var state = new SelectionState
            {
                SelectedKeys = allKeys,
                AllEntireDatasetSelected = _allEntireDatasetSelected
            };
            await LocalStorage.SetAsync(SelectionStorageKey, state);
        }
        catch { }
    }

    private async Task RestoreSelectionAsync()
    {
        if (string.IsNullOrEmpty(TableId) || SelectionMode == SelectionMode.None) return;
        try
        {
            var state = await LocalStorage.GetAsync<SelectionState>(SelectionStorageKey);
            if (state is null) return;

            _allEntireDatasetSelected = state.AllEntireDatasetSelected;

            if (state.SelectedKeys.Count > 0)
            {
                _restoredSelectedKeys = new HashSet<string>(state.SelectedKeys);
            }

            UpdatePageSelectionState();
        }
        catch { }
    }

    private void SyncSelectedItems()
    {
        if (_displayItems.Count == 0) return;

        bool changed = false;
        foreach (var item in _displayItems)
        {
            var key = GetItemKey(item);
            var keyStr = key?.ToString() ?? "";

            if (_allEntireDatasetSelected)
            {
                if (key != null && !_selectedItems.ContainsKey(key))
                {
                    _selectedItems[key] = item;
                    changed = true;
                }
            }
            else if (key != null && _restoredSelectedKeys != null && _restoredSelectedKeys.Contains(keyStr))
            {
                if (!_selectedItems.ContainsKey(key))
                {
                    _selectedItems[key] = item;
                    _restoredSelectedKeys.Remove(keyStr);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            UpdatePageSelectionState();
        }
    }

    private class SelectionState
    {
        public List<string> SelectedKeys { get; set; } = new();
        public bool AllEntireDatasetSelected { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // FILTER SIDEBAR
    // ═══════════════════════════════════════════════════════════

    private void ToggleFilterSidebar()
    {
        _filterSidebarOpen = !_filterSidebarOpen;
        if (_filterSidebarOpen) InitializeStaging();
    }

    private void InitializeStaging()
    {
        _stagedFilters = _request.Filters.Select(f => new FilterDefinition
        {
            Field = f.Field,
            Type = f.Type,
            Operator = f.Operator,
            Value = f.Value,
            ValueTo = f.ValueTo
        }).ToList();
    }

    private void CloseFilterSidebar() => _filterSidebarOpen = false;

    private async Task ApplyFiltersAsync()
    {
        _request.Filters.Clear();
        _request.Filters.AddRange(_stagedFilters);
        _request.PageNumber = 1;
        CloseFilterSidebar();
        await RefreshAsync();
    }

    private async Task ClearAllFiltersAsync()
    {
        _stagedFilters.Clear();
        _request.Filters.Clear();
        _request.PageNumber = 1;
        CloseFilterSidebar();
        await RefreshAsync();
    }

    private string? GetStagedFilterValue(string field) =>
        _stagedFilters.FirstOrDefault(f => f.Field == field)?.Value;

    private void SetStagedFilter(string field, string? value, FilterType type,
        FilterOperator op = FilterOperator.Contains, string? valueTo = null)
    {
        var existing = _stagedFilters.FirstOrDefault(f => f.Field == field);
        if (existing is not null) _stagedFilters.Remove(existing);

        if (!string.IsNullOrEmpty(value))
        {
            _stagedFilters.Add(new FilterDefinition
            {
                Field = field,
                Type = type,
                Operator = op,
                Value = value,
                ValueTo = valueTo
            });
        }
    }

    // ═══════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════

    private string GetRowCss(TItem item)
    {
        var parts = new List<string>(3) { "dt-row" };
        if (IsSelected(item)) parts.Add("dt-row-selected");
        if (SelectionMode != SelectionMode.None || OnRowClick.HasDelegate) parts.Add("dt-row-clickable");
        var custom = RowClass?.Invoke(item);
        if (!string.IsNullOrEmpty(custom)) parts.Add(custom);
        return string.Join(' ', parts);
    }

    private string GetCellInlineStyle(ColumnDefinition<TItem> col, TItem item)
    {
        var parts = new List<string>(2);
        var colStyle = col.GetColumnStyle();
        if (colStyle.Length > 0) parts.Add(colStyle);
        var custom = col.CellStyle?.Invoke(item);
        if (!string.IsNullOrEmpty(custom)) parts.Add(custom);
        return string.Join(';', parts);
    }

    // ═══════════════════════════════════════════════════════════
    // DISPOSE
    // ═══════════════════════════════════════════════════════════

    private void OnThemeChanged(LHA.BlazorWasm.Services.Theme.CThemeMode _) =>
        InvokeAsync(StateHasChanged);

    public override void Dispose()
    {
        base.Dispose();
        ThemeState.OnThemeChanged -= OnThemeChanged;
        _searchDebouncer?.Dispose();
        _filterDebouncer?.Dispose();
    }
}
