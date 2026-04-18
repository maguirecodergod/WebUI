using LHA.BlazorWasm.Components.Dialog;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Users;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Identity;

public partial class UserPickerModal : LhaComponentBase
{
    [Inject] private UserApiClient UserAppService { get; set; } = default!;

    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public EventCallback<List<IdentityUserDto>> OnUsersSelected { get; set; }
    [Parameter] public List<Guid> ExcludeIds { get; set; } = [];

    private PremiumDialog _dialog = default!;
    private bool _isVisible;
    private bool _isLoading;
    private string _searchText = string.Empty;
    private List<IdentityUserDto> _users = [];
    private HashSet<Guid> _selectedIds = [];
    private List<IdentityUserDto> _selectedUsers = [];

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Title))
        {
            Title = L("Roles.SelectUsers");
        }
    }

    public async Task OpenAsync(List<Guid>? assignedIds = null)
    {
        _selectedIds = assignedIds?.ToHashSet() ?? new HashSet<Guid>();
        _selectedUsers.Clear(); // We'll populate this if needed, but ConfirmSelection handles it
        _searchText = string.Empty;
        _isVisible = true;
        await LoadUsersAsync();
        
        // Populate _selectedUsers with users already in the loaded list that are selected
        UpdateSelectedUsersList();

        await _dialog.OpenAsync();
    }

    private async Task LoadUsersAsync()
    {
        _isLoading = true;
        try
        {
            var result = await UserAppService.GetListAsync(new GetIdentityUsersInput
            {
                Filter = _searchText,
                PageSize = 20 // Keep it manageable
            });

            _users = result?.Items?.ToList() ?? [];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnSearchChanged(string val)
    {
        _searchText = val;
        await LoadUsersAsync();
    }

    private void ToggleUser(IdentityUserDto user, bool isSelected)
    {
        if (isSelected)
        {
            _selectedIds.Add(user.Id);
        }
        else
        {
            _selectedIds.Remove(user.Id);
        }
        UpdateSelectedUsersList();
    }

    private void UpdateSelectedUsersList()
    {
        // This is a bit tricky with paged data. 
        // We only return what we have in the current view or have tracked.
        // Actually, it's safer to just return the IDs or ensure we have the objects.
        foreach (var user in _users)
        {
            if (_selectedIds.Contains(user.Id))
            {
                if (!_selectedUsers.Any(u => u.Id == user.Id))
                    _selectedUsers.Add(user);
            }
            else
            {
                var item = _selectedUsers.FirstOrDefault(u => u.Id == user.Id);
                if (item != null) _selectedUsers.Remove(item);
            }
        }
    }

    private bool IsSelected(Guid id) => _selectedIds.Contains(id);

    private async Task ConfirmSelectionAsync()
    {
        if (_selectedUsers.Any())
        {
            await OnUsersSelected.InvokeAsync(_selectedUsers);
        }
        await _dialog.CloseAsync();
    }
}
