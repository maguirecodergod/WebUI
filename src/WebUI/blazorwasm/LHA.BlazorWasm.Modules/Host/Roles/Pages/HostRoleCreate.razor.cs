using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Tabs;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.BlazorWasm.HttpApi.Client.Clients.PermissionManagement;
using LHA.BlazorWasm.Modules.Host.Roles.Models;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Permissions;
using LHA.Shared.Contracts.Identity.Roles;
using LHA.Shared.Contracts.Identity.Users;
using LHA.Shared.Contracts.PermissionManagement;
using LHA.Shared.Domain.Identity;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Modules.Host.Roles.Pages;

public partial class HostRoleCreate : LHAComponentBase
{
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;
    [Inject] private UserApiClient UserAppService { get; set; } = default!;
    [Inject] private PermissionGroupApiClient PermissionGroupAppService { get; set; } = default!;
    [Inject] private PermissionApiClient PermissionAppService { get; set; } = default!;

    // ── Wizard State ────────────────────────────────────────────────
    private int _currentStep = 0;
    private const int TotalSteps = 3;
    private bool _isCreating;
    private bool _isCompleted;
    private Guid? _createdRoleId;

    // ── Step 1: Role Info ───────────────────────────────────────────
    private string _roleName = string.Empty;
    private bool _isDefault;
    private bool _isPublic = true;
    private string? _nameError;

    // ── Step 2: Permissions ─────────────────────────────────────────
    private List<PermissionGroupViewModel> _groups = [];
    private string _permissionFilter = string.Empty;
    private bool _isLoadingPermissions;

    // ── Step 3: Users ───────────────────────────────────────────────
    private List<IdentityUserDto> _allUsers = [];
    private List<IdentityUserDto> _filteredUsers = [];
    private List<IdentityUserDto> _selectedUsers = [];
    private string _userSearchQuery = string.Empty;
    private bool _isLoadingUsers;

    // ── Breadcrumb ──────────────────────────────────────────────────
    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems =
        [
            new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
            new() { Text = L("Menu.Host.Roles"), Href = "/host/roles", Icon = "bi bi-shield-lock" },
            new() { Text = L("Common.Create"), Icon = "bi bi-plus-circle" }
        ];

        await LoadPermissionDefinitionsAsync();
    }

    // ── Step Navigation ─────────────────────────────────────────────

    private bool CanGoNext => _currentStep switch
    {
        0 => !string.IsNullOrWhiteSpace(_roleName),
        1 => true,
        2 => true,
        _ => false
    };

    private async Task NextStepAsync()
    {
        if (_currentStep == 0)
        {
            _nameError = string.IsNullOrWhiteSpace(_roleName)
                ? L("Roles.NameRequired")
                : null;
            if (_nameError != null) return;
        }

        if (_currentStep == 1 && _allUsers.Count == 0)
        {
            await LoadAvailableUsersAsync();
        }

        if (_currentStep < TotalSteps - 1)
        {
            _currentStep++;
            StateHasChanged();
        }
    }

    private void PreviousStep()
    {
        if (_currentStep > 0)
        {
            _currentStep--;
            StateHasChanged();
        }
    }

    // ── Data Loading ────────────────────────────────────────────────

    private async Task LoadPermissionDefinitionsAsync()
    {
        _isLoadingPermissions = true;
        try
        {
            var groupsRes = await PermissionGroupAppService.GetListAsync(
                new GetPermissionGroupsInput { PageSize = 100 });
            if (groupsRes?.Items != null)
            {
                _groups = groupsRes.Items.Select(g => new PermissionGroupViewModel
                {
                    Name = g.Name,
                    DisplayName = L(g.DisplayName),
                    ServiceName = g.ServiceName,
                    Permissions = g.Permissions.Select(p => new PermissionDefinitionViewModel
                    {
                        Name = p.Name,
                        DisplayName = L(p.DisplayName)
                    }).ToList()
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
        finally
        {
            _isLoadingPermissions = false;
        }
    }

    private async Task LoadAvailableUsersAsync()
    {
        _isLoadingUsers = true;
        try
        {
            var usersRes = await UserAppService.GetListAsync(
                new GetIdentityUsersInput { PageSize = 1000 });
            _allUsers = usersRes?.Items?.ToList() ?? [];
            _filteredUsers = _allUsers;
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
        finally
        {
            _isLoadingUsers = false;
        }
    }

    // ── Permission Handling ─────────────────────────────────────────

    private void OnPermissionChanged(PermissionDefinitionViewModel p, bool granted)
    {
        p.IsGranted = granted;
        StateHasChanged();
    }

    private void ToggleAllGroupPermissions(PermissionGroupViewModel group, bool granted)
    {
        foreach (var p in group.Permissions)
        {
            p.IsGranted = granted;
        }
        StateHasChanged();
    }

    private bool AreAllGroupPermissionsGranted(PermissionGroupViewModel group)
        => group.Permissions.All(p => p.IsGranted);

    private int GetGrantedPermissionCount()
        => _groups.SelectMany(g => g.Permissions).Count(p => p.IsGranted);

    // ── User Selection ──────────────────────────────────────────────

    private void SearchUsers(string query)
    {
        _userSearchQuery = query;
        if (string.IsNullOrWhiteSpace(query))
        {
            _filteredUsers = _allUsers
                .Where(u => !_selectedUsers.Any(s => s.Id == u.Id))
                .ToList();
        }
        else
        {
            _filteredUsers = _allUsers
                .Where(u => !_selectedUsers.Any(s => s.Id == u.Id))
                .Where(u => u.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            (u.Email != null && u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }

    private void AddUserToSelection(IdentityUserDto user)
    {
        if (_selectedUsers.All(u => u.Id != user.Id))
        {
            _selectedUsers.Add(user);
            _filteredUsers = _filteredUsers.Where(u => u.Id != user.Id).ToList();
            StateHasChanged();
        }
    }

    private void RemoveUserFromSelection(IdentityUserDto user)
    {
        _selectedUsers.RemoveAll(u => u.Id == user.Id);
        // Re-apply filter
        SearchUsers(_userSearchQuery);
        StateHasChanged();
    }

    // ── Final Submit ────────────────────────────────────────────────

    private async Task SubmitAsync()
    {
        _nameError = string.IsNullOrWhiteSpace(_roleName)
            ? L("Roles.NameRequired")
            : null;
        if (_nameError != null)
        {
            _currentStep = 0;
            return;
        }

        _isCreating = true;
        try
        {
            // Step 1: Create the role
            var createdRole = await RoleAppService.CreateAsync(new CreateIdentityRoleInput
            {
                Name = _roleName,
                IsDefault = _isDefault,
                IsPublic = _isPublic
            });

            if (createdRole == null)
            {
                ToastNotification.Error(L("Roles.CreateFailed"));
                return;
            }

            _createdRoleId = createdRole.Id;

            // Step 2: Set permissions
            var permissions = _groups
                .SelectMany(g => g.Permissions)
                .Select(p => new PermissionGrantInput { Name = p.Name, IsGranted = p.IsGranted })
                .ToList();

            if (permissions.Any(p => p.IsGranted))
            {
                await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
                {
                    ProviderName = PermissionGrantProviderName.Role,
                    ProviderKey = createdRole.Id.ToString().ToLowerInvariant(),
                    Permissions = permissions
                });
            }

            // Step 3: Assign users
            foreach (var user in _selectedUsers)
            {
                try
                {
                    var userRolesRes = await UserAppService.GetRolesAsync(user.Id);
                    var roleIds = userRolesRes?.Select(r => r.Id).ToList() ?? [];
                    if (!roleIds.Contains(createdRole.Id))
                    {
                        roleIds.Add(createdRole.Id);
                        await UserAppService.UpdateRolesAsync(user.Id, roleIds);
                    }
                }
                catch (Exception ex)
                {
                    ToastNotification.Error($"{L("Roles.UserAssignFailed")}: {user.UserName} - {ex.Message}");
                }
            }

            _isCompleted = true;
            ToastNotification.Success(L("Roles.CreateSuccess"));
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Roles.CreateFailed")}: {ex.Message}");
        }
        finally
        {
            _isCreating = false;
        }
    }

    private void NavigateToCreatedRole()
    {
        if (_createdRoleId.HasValue)
            Navigation.NavigateTo($"/host/roles/{_createdRoleId.Value}");
        else
            Navigation.NavigateTo("/host/roles");
    }

    private void NavigateToRoleList() => Navigation.NavigateTo("/host/roles");
}
