using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Select;

public partial class Select<TValue> : LhaComponentBase
{
    private IJSObjectReference? _jsModule;
    private ElementReference _selectRef;

    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    [Parameter] public IEnumerable<TValue>? Values { get; set; }
    [Parameter] public EventCallback<IEnumerable<TValue>?> ValuesChanged { get; set; }

    [Parameter] public ICollection<SelectOption<TValue>> Options { get; set; } = new List<SelectOption<TValue>>();
    [Parameter] public string Placeholder { get; set; } = "Select...";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public SelectMode Mode { get; set; } = SelectMode.Single;

    [Parameter] public bool Searchable { get; set; }
    [Parameter] public bool AsyncSearch { get; set; }
    [Parameter] public Func<string, Task<IEnumerable<SelectOption<TValue>>>>? LoadOptions { get; set; }

    /// <summary>
    /// Callback for server-side paging. (startIndex, count, searchText) => (Items, TotalCount)
    /// </summary>
    [Parameter] public Func<int, int, string?, Task<(IEnumerable<SelectOption<TValue>> Items, int TotalCount)>>? OnLoadPage { get; set; }

    [Parameter] public string MaxDropdownHeight { get; set; } = "300px";
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool ShowClear { get; set; } = true;

    [Parameter] public RenderFragment<SelectOption<TValue>>? OptionTemplate { get; set; }
    [Parameter] public RenderFragment<TValue?>? ValueTemplate { get; set; }
    [Parameter] public EventCallback OnChange { get; set; }
    [Parameter] public SelectPlacement Placement { get; set; } = SelectPlacement.Auto;

    protected SelectState<TValue> State { get; } = new();
    private bool _isOpeningUpwards;
    private List<SelectOption<TValue>> _internalOptions = new();
    private List<SelectOption<TValue>> _filteredOptions = new();

    protected override void OnInitialized()
    {
        OptionTemplate ??= (option) => builder => builder.AddContent(0, option.Label);
        State.ItemsProvider = SelectItemsProvider;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!AsyncSearch)
        {
            _internalOptions = Options?.ToList() ?? new List<SelectOption<TValue>>();
            FilterOptions();
        }
        else if (State.IsOpen && string.IsNullOrEmpty(State.SearchText) && !_internalOptions.Any())
        {
            await HandleSearch(string.Empty);
        }
    }

    private bool IsSingleMode => Mode == SelectMode.Single;
    private bool HasValue => Value != null;
    private bool HasAnyValue => IsSingleMode ? HasValue : (Values?.Any() ?? false);

    private ICollection<SelectOption<TValue>> FilteredOptions => _filteredOptions;

    private void FilterOptions()
    {
        if (string.IsNullOrWhiteSpace(State.SearchText))
        {
            _filteredOptions = _internalOptions;
        }
        else
        {
            _filteredOptions = _internalOptions
                .Where(o => o.Label.Contains(State.SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        State.NotifyOptionsChanged();
    }

    private async Task HandleSearch(string text)
    {
        State.SearchText = text;
        State.FocusedIndex = -1;
        State.NotifyOptionsChanged();

        if (AsyncSearch && LoadOptions != null)
        {
            State.IsLoading = true;
            StateHasChanged();
            try
            {
                var results = await LoadOptions(text);
                _internalOptions = results?.ToList() ?? new List<SelectOption<TValue>>();
                _filteredOptions = _internalOptions;
            }
            finally
            {
                State.IsLoading = false;
            }
        }
        else
        {
            FilterOptions();
        }

        StateHasChanged();
    }

    private async Task ToggleDropdown()
    {
        if (Disabled || ReadOnly) return;
        HandleInternalInteraction();

        if (!State.IsOpen)
        {
            if (Placement == SelectPlacement.Top)
            {
                _isOpeningUpwards = true;
            }
            else if (Placement == SelectPlacement.Bottom)
            {
                _isOpeningUpwards = false;
            }
            else // Auto
            {
                _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./_content/LHA.BlazorWasm.Components/Select/Select.razor.js");
                _isOpeningUpwards = await _jsModule.InvokeAsync<bool>("shouldOpenUpwards", _selectRef);
            }
        }

        State.IsOpen = !State.IsOpen;
        if (State.IsOpen)
        {
            State.FocusedIndex = -1;
        }
    }

    private async Task SelectItemAsync(SelectOption<TValue> option)
    {
        if (option.Disabled) return;

        if (IsSingleMode)
        {
            Value = option.Value;
            await ValueChanged.InvokeAsync(Value);
            State.IsOpen = false;
        }
        else
        {
            var currentValues = Values?.ToList() ?? new List<TValue>();
            if (currentValues.Contains(option.Value))
            {
                currentValues.Remove(option.Value);
            }
            else
            {
                currentValues.Add(option.Value);
            }
            Values = currentValues;
            await ValuesChanged.InvokeAsync(Values);
        }

        await OnChange.InvokeAsync();
        StateHasChanged();
    }

    private async Task RemoveValue(TValue val)
    {
        if (Disabled || ReadOnly) return;
        var currentValues = Values?.ToList() ?? new List<TValue>();
        currentValues.Remove(val);
        Values = currentValues;
        await ValuesChanged.InvokeAsync(Values);
        await OnChange.InvokeAsync();
    }

    private async Task ClearAsync()
    {
        if (IsSingleMode)
        {
            Value = default;
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            Values = Enumerable.Empty<TValue>();
            await ValuesChanged.InvokeAsync(Values);
        }
        await OnChange.InvokeAsync();
    }

    private string GetLabel(TValue? val)
    {
        if (val == null) return string.Empty;
        var opt = _internalOptions.FirstOrDefault(o => EqualityComparer<TValue>.Default.Equals(o.Value, val));
        return opt?.Label ?? val.ToString() ?? string.Empty;
    }

    private bool IsOptionSelected(SelectOption<TValue> option)
    {
        if (IsSingleMode)
        {
            return EqualityComparer<TValue>.Default.Equals(Value, option.Value);
        }
        return Values?.Contains(option.Value) ?? false;
    }

    private bool IsOptionFocused(SelectOption<TValue> option)
    {
        if (State.FocusedIndex < 0 || State.FocusedIndex >= _filteredOptions.Count) return false;
        return _filteredOptions[State.FocusedIndex] == option;
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (Disabled || ReadOnly) return;

        switch (e.Key)
        {
            case "ArrowDown":
                if (!State.IsOpen)
                {
                    State.IsOpen = true;
                }
                else
                {
                    State.FocusedIndex = Math.Min(State.FocusedIndex + 1, _filteredOptions.Count - 1);
                }
                break;
            case "ArrowUp":
                if (State.IsOpen)
                {
                    State.FocusedIndex = Math.Max(State.FocusedIndex - 1, 0);
                }
                break;
            case "Enter":
                if (State.IsOpen && State.FocusedIndex >= 0 && State.FocusedIndex < _filteredOptions.Count)
                {
                    await SelectItemAsync(_filteredOptions[State.FocusedIndex]);
                }
                else if (!State.IsOpen)
                {
                    State.IsOpen = true;
                }
                break;
            case "Escape":
                State.IsOpen = false;
                break;
        }
    }

    private DateTime _lastInteractionTime = DateTime.MinValue;

    protected void HandleInternalInteraction()
    {
        _lastInteractionTime = DateTime.Now;
    }

    protected async Task OnFocusOut()
    {
        await Task.Delay(300);

        if ((DateTime.Now - _lastInteractionTime).TotalMilliseconds < 500)
        {
            return;
        }

        if (State.IsOpen)
        {
            State.IsOpen = false;
            StateHasChanged();
        }
    }

    private async ValueTask<ItemsProviderResult<SelectOption<TValue>>> SelectItemsProvider(ItemsProviderRequest request)
    {
        if (OnLoadPage != null)
        {
            try
            {
                var response = await OnLoadPage(request.StartIndex, request.Count, State.SearchText);
                return new ItemsProviderResult<SelectOption<TValue>>(response.Items, response.TotalCount);
            }
            catch (Exception)
            {
                return new ItemsProviderResult<SelectOption<TValue>>(Enumerable.Empty<SelectOption<TValue>>(), 0);
            }
        }

        var numOptions = Math.Min(request.Count, _filteredOptions.Count - request.StartIndex);
        var options = _filteredOptions.Skip(request.StartIndex).Take(numOptions);

        return new ItemsProviderResult<SelectOption<TValue>>(options, _filteredOptions.Count);
    }
}
