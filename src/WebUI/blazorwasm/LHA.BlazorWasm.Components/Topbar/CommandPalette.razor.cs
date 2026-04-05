using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Topbar;

public partial class CommandPalette : LhaComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }



    private ElementReference _searchInput;
    private string _searchText = string.Empty;
    private List<SearchResultItem> _results = new();
    private int _activeIndex = -1;

    private List<SearchResultItem> Suggestions => _results.Where(x => string.IsNullOrEmpty(x.Category) || x.Category == "Suggestions").ToList();

    private List<SearchResultItem> FilteredResults => string.IsNullOrWhiteSpace(_searchText)
        ? new List<SearchResultItem>()
        : _results.Where(x => x.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                             (x.Description?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private void LoadInitialResults()
    {
        _results = new List<SearchResultItem>
        {
            new SearchResultItem { Title = L("Search.GoToHome"), Description = L("Search.GoToHomeDesc"), Icon = "bi bi-house", Url = "/", Shortcut = "G H" },
            new SearchResultItem { Title = L("Search.MyProfile"), Description = L("Search.MyProfileDesc"), Icon = "bi bi-person", Url = "/profile", Shortcut = "G P" },
            new SearchResultItem { Title = L("Search.Settings"), Description = L("Search.SettingsDesc"), Icon = "bi bi-gear", Url = "/settings", Shortcut = "G S" },
            new SearchResultItem { Title = L("Search.Notifications"), Description = L("Search.NotificationsDesc"), Icon = "bi bi-bell", Url = "/notifications", Shortcut = "G N" },
            new SearchResultItem { Title = L("Search.ChangeTheme"), Description = L("Search.ChangeThemeDesc"), Icon = "bi bi-palette", Category = "Appearance" },
            new SearchResultItem { Title = L("Search.ChangeLanguage"), Description = L("Search.ChangeLanguageDesc"), Icon = "bi bi-globe", Category = "Appearance" },
            new SearchResultItem { Title = L("Search.Logout"), Description = L("Search.LogoutDesc"), Icon = "bi bi-box-arrow-right", Url = "/logout", Category = "System" }
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            LoadInitialResults();
            _searchText = string.Empty;
            _activeIndex = 0;
            UpdateActiveItem();
            await Task.Delay(50); // Wait for render
            try 
            {
                await _searchInput.FocusAsync();
            }
            catch (JSException) { /* Ignored */ }
        }
    }

    private void HandleInput(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        _activeIndex = 0;
        UpdateActiveItem();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        var currentList = string.IsNullOrWhiteSpace(_searchText) ? Suggestions : FilteredResults;
        if (!currentList.Any()) return;

        if (e.Key == "ArrowDown")
        {
            _activeIndex = (_activeIndex + 1) % currentList.Count;
            UpdateActiveItem();
        }
        else if (e.Key == "ArrowUp")
        {
            _activeIndex = (_activeIndex - 1 + currentList.Count) % currentList.Count;
            UpdateActiveItem();
        }
        else if (e.Key == "Enter")
        {
            await SelectItem(currentList[_activeIndex]);
        }
        else if (e.Key == "Escape")
        {
            await ClosePalette();
        }
    }

    private void UpdateActiveItem()
    {
        var currentList = string.IsNullOrWhiteSpace(_searchText) ? Suggestions : FilteredResults;
        for (int i = 0; i < currentList.Count; i++)
        {
            currentList[i].IsActive = (i == _activeIndex);
        }
    }

    private async Task SelectItem(SearchResultItem item)
    {
        if (item.OnSelect != null)
        {
            item.OnSelect.Invoke();
        }
        else if (!string.IsNullOrEmpty(item.Url))
        {
            Navigation.NavigateTo(item.Url);
        }

        await ClosePalette();
    }

    private async Task ClosePalette()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(false);
    }
}
