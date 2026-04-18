using LHA.BlazorWasm.Components.Dialog;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Roles;
using LHA.Shared.Contracts.Identity.Permissions;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Identity;

namespace LHA.BlazorWasm.Modules.Host.Roles.Components;

public partial class HostRoleCreateEditModal : LhaComponentBase
{
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;
    [Inject] private PermissionApiClient PermissionAppService { get; set; } = default!;
    [Inject] private UserApiClient UserAppService { get; set; } = default!;
    [Inject] private LHA.BlazorWasm.HttpApi.Client.Clients.PermissionManagement.PermissionGroupApiClient PermissionGroupAppService { get; set; } = default!;

    [Parameter] public EventCallback OnSaved { get; set; }

    private PremiumDialog _dialog = default!;
    private bool _isVisible;
    private bool _isEdit;
    private Guid _roleId;

    private RoleFormModel _role = new();
    private List<PermissionGroupViewModel> _groups = [];
    private List<IdentityUserDto> _assignedUsers = [];
    private string _permissionFilter = string.Empty;

    // User-specific permissions handling
    private IdentityUserDto? _selectedUser;
    private List<PermissionGroupViewModel> _userGroups = [];
    private bool _isUserPermissionsVisible;
    private string _userPermissionFilter = string.Empty;
    private UserPickerModal _userPicker = default!;
    private RoleTab _activeTab = RoleTab.General;

    private enum RoleTab { General, Users }

    public async Task ShowCreateModal()
    {
        _isEdit = false;
        _roleId = Guid.Empty;
        _role = new RoleFormModel();
        _assignedUsers = [];
        _activeTab = RoleTab.General;

        await LoadPermissionsAsync();

        await _dialog.OpenAsync();
    }

    public async Task ShowEditModal(Guid roleId)
    {
        _isEdit = true;
        _roleId = roleId;
        _activeTab = RoleTab.General;

        var dto = await RoleAppService.GetAsync(roleId);
        if (dto != null)
        {
            _role = new RoleFormModel
            {
                Name = dto.Name,
                IsDefault = dto.IsDefault,
                IsPublic = dto.IsPublic,
                ConcurrencyStamp = dto.ConcurrencyStamp
            };

            await LoadPermissionsAsync(dto.Name);

            // Fetch assigned users
            var usersResult = await UserAppService.GetListAsync(new LHA.Shared.Contracts.Identity.Users.GetIdentityUsersInput
            {
                RoleId = roleId,
                PageSize = 5 // Just show a few for the modal
            });
            _assignedUsers = usersResult?.Items?.ToList() ?? [];
        }

        await _dialog.OpenAsync();
    }

    private async Task LoadPermissionsAsync(string? roleName = null)
    {
        // 1. Fetch all definitions (Groups -> Permissions)
        var groupsResponse = await PermissionGroupAppService.GetListAsync(new LHA.Shared.Contracts.PermissionManagement.GetPermissionGroupsInput { PageSize = 100 });
        if (groupsResponse != null)
        {
            _groups = groupsResponse.Items
                .Select(g => new PermissionGroupViewModel
                {
                    Name = g.Name,
                    DisplayName = g.DisplayName,
                    Permissions = g.Permissions
                        .Where(p => PermissionService.HasPermission(p.Name)) // Security: Only show what user can grant
                        .Select(p => new PermissionDefinitionViewModel
                        {
                            Name = p.Name,
                            DisplayName = p.DisplayName,
                            IsGranted = false
                        }).ToList()
                })
                .Where(g => g.Permissions.Any())
                .ToList();

            // 2. Fetch grants and patch
            if (!string.IsNullOrEmpty(roleName))
            {
                var permResult = await PermissionAppService.GetAsync(new GetPermissionListInput { ProviderName = "R", ProviderKey = roleName });
                if (permResult != null)
                {
                    var grantedNames = permResult.Where(x => x.IsGranted).Select(x => x.Name).ToHashSet();
                    foreach (var group in _groups)
                    {
                        foreach (var p in group.Permissions)
                        {
                            if (grantedNames.Contains(p.Name))
                            {
                                p.IsGranted = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private async Task ShowUserPermissionsAsync(IdentityUserDto user)
    {
        _selectedUser = user;
        _isUserPermissionsVisible = true;

        // Load definitions and user-specific grants
        var groupsResponse = await PermissionGroupAppService.GetListAsync(new LHA.Shared.Contracts.PermissionManagement.GetPermissionGroupsInput { PageSize = 100 });
        if (groupsResponse != null)
        {
            _userGroups = groupsResponse.Items
                .Select(g => new PermissionGroupViewModel
                {
                    Name = g.Name,
                    DisplayName = g.DisplayName,
                    Permissions = g.Permissions
                        .Where(p => PermissionService.HasPermission(p.Name)) // Security: Only show what user can grant
                        .Select(p => new PermissionDefinitionViewModel
                        {
                            Name = p.Name,
                            DisplayName = p.DisplayName,
                            IsGranted = false
                        }).ToList()
                })
                .Where(g => g.Permissions.Any())
                .ToList();

            var permResult = await PermissionAppService.GetAsync(new GetPermissionListInput { ProviderName = "U", ProviderKey = user.Id.ToString() });
            var userGrants = permResult?.Where(x => x.IsGranted).Select(x => x.Name).ToHashSet() ?? new HashSet<string>();

            // Fetch role-inherited permissions
            var userRoles = await UserAppService.GetRolesAsync(user.Id);
            var roleGrants = new HashSet<string>();
            var roleMap = new Dictionary<string, List<string>>(); // PermissionName -> List of RoleNames

            if (userRoles != null)
            {
                foreach (var role in userRoles)
                {
                    var rolePerms = await PermissionAppService.GetAsync(new GetPermissionListInput { ProviderName = "R", ProviderKey = role.Name });
                    if (rolePerms != null)
                    {
                        foreach (var rp in rolePerms.Where(x => x.IsGranted))
                        {
                            roleGrants.Add(rp.Name);
                            if (!roleMap.ContainsKey(rp.Name)) roleMap[rp.Name] = new List<string>();
                            roleMap[rp.Name].Add(role.Name);
                        }
                    }
                }
            }

            foreach (var group in _userGroups)
            {
                foreach (var p in group.Permissions)
                {
                    p.IsInherited = roleGrants.Contains(p.Name);
                    p.InheritedFrom = roleMap.ContainsKey(p.Name) ? roleMap[p.Name] : new List<string>();
                    p.IsGranted = p.IsInherited || userGrants.Contains(p.Name);
                }
            }
        }
    }



    private IEnumerable<PermissionGroupViewModel> FilteredUserGroups
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_userPermissionFilter))
                return _userGroups;

            return _userGroups.Select(g => new PermissionGroupViewModel
            {
                Name = g.Name,
                DisplayName = g.DisplayName,
                Permissions = g.Permissions.Where(p =>
                    p.DisplayName.Contains(_userPermissionFilter, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(_userPermissionFilter, StringComparison.OrdinalIgnoreCase)).ToList()
            }).Where(g => g.Permissions.Any());
        }
    }



    private IEnumerable<PermissionGroupViewModel> FilteredGroups
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_permissionFilter))
                return _groups;

            return _groups.Select(g => new PermissionGroupViewModel
            {
                Name = g.Name,
                DisplayName = g.DisplayName,
                Permissions = g.Permissions.Where(p =>
                    p.DisplayName.Contains(_permissionFilter, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(_permissionFilter, StringComparison.OrdinalIgnoreCase)).ToList()
            }).Where(g => g.Permissions.Any());
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (_isEdit)
            {
                await RoleAppService.UpdateAsync(_roleId, new UpdateIdentityRoleInput
                {
                    Name = _role.Name,
                    IsDefault = _role.IsDefault,
                    IsPublic = _role.IsPublic,
                    ConcurrencyStamp = _role.ConcurrencyStamp
                });

                if (_groups.Any())
                {
                    await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
                    {
                        ProviderName = "R",
                        ProviderKey = _role.Name,
                        Permissions = MapPermissions(_groups)
                    });
                }
            }
            else
            {
                var createdRole = await RoleAppService.CreateAsync(new CreateIdentityRoleInput
                {
                    Name = _role.Name,
                    IsDefault = _role.IsDefault,
                    IsPublic = _role.IsPublic
                });

                if (createdRole != null && _groups.Any())
                {
                    await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
                    {
                        ProviderName = "R",
                        ProviderKey = createdRole.Name,
                        Permissions = MapPermissions(_groups)
                    });
                }
            }

            ToastNotification.Success(L("Common.SavedSuccessfully"));
            await _dialog.CloseAsync();
            await OnSaved.InvokeAsync();
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Common.SaveFailed") + ": " + ex.Message);
        }
    }

    private async Task SaveUserPermissionsAsync()
    {
        if (_selectedUser == null || !_userGroups.Any()) return;

        try
        {
            await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
            {
                ProviderName = "U",
                ProviderKey = _selectedUser.Id.ToString(),
                Permissions = MapPermissions(_userGroups)
            });

            ToastNotification.Success(L("Common.SavedSuccessfully"));
            _isUserPermissionsVisible = false;
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Common.SaveFailed") + ": " + ex.Message);
        }
    }

    private void ToggleGroup(string groupName, bool isGranted) => ToggleGroupInternal(groupName, isGranted, _groups);
    private bool IsGroupAllSelected(string groupName) => IsGroupAllSelectedInternal(groupName, _groups);
    private void ToggleUserGroup(string groupName, bool isGranted) => ToggleGroupInternal(groupName, isGranted, _userGroups);
    private bool IsUserGroupAllSelected(string groupName) => IsGroupAllSelectedInternal(groupName, _userGroups);

    private void ToggleGroupInternal(string groupName, bool isGranted, List<PermissionGroupViewModel> groups)
    {
        var group = groups.FirstOrDefault(g => g.Name == groupName);
        if (group != null)
        {
            foreach (var p in group.Permissions) p.IsGranted = isGranted;
        }
    }

    private bool IsGroupAllSelectedInternal(string groupName, List<PermissionGroupViewModel> groups)
    {
        var group = groups.FirstOrDefault(g => g.Name == groupName);
        return group != null && group.Permissions.Any() && group.Permissions.All(p => p.IsGranted);
    }

    private List<PermissionGrantInput> MapPermissions(List<PermissionGroupViewModel> groups)
    {
        return groups.SelectMany(g => g.Permissions)
            .Select(p => new PermissionGrantInput { Name = p.Name, IsGranted = p.IsGranted })
            .ToList();
    }

    private async Task RemoveUserFromRoleAsync(IdentityUserDto user)
    {
        try
        {
            var userRoles = await UserAppService.GetRolesAsync(user.Id);
            if (userRoles != null)
            {
                var newRoleIds = userRoles.Where(r => r.Id != _roleId).Select(r => r.Id).ToList();
                await UserAppService.UpdateRolesAsync(user.Id, newRoleIds);

                // Refresh list
                _assignedUsers.Remove(user);
                ToastNotification.Success(L("Roles.UserRemovedSuccessfully", user.UserName));
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Roles.UserRemoveFailed") + ": " + ex.Message);
        }
    }

    private void OpenUserPicker()
    {
        var existingIds = _assignedUsers.Select(u => u.Id).ToList();
        _userPicker.OpenAsync(existingIds);
    }

    private async Task AddUsersToRoleAsync(List<IdentityUserDto> selectedUsers)
    {
        try
        {
            var selectedIds = selectedUsers.Select(u => u.Id).ToHashSet();
            var currentAssignedIds = _assignedUsers.Select(u => u.Id).ToHashSet();

            // 1. Handle Removals (Users in current list but NOT in new selection)
            var usersToRemove = _assignedUsers.Where(u => !selectedIds.Contains(u.Id)).ToList();
            foreach (var user in usersToRemove)
            {
                await RemoveUserFromRoleAsync(user);
            }

            // 2. Handle Additions (Users in new selection but NOT in current list)
            var usersToAdd = selectedUsers.Where(u => !currentAssignedIds.Contains(u.Id)).ToList();
            foreach (var user in usersToAdd)
            {
                if (_roleId != Guid.Empty)
                {
                    var userRoles = await UserAppService.GetRolesAsync(user.Id);
                    var roleIds = userRoles?.Select(r => r.Id).ToList() ?? new List<Guid>();
                    if (!roleIds.Contains(_roleId))
                    {
                        roleIds.Add(_roleId);
                        await UserAppService.UpdateRolesAsync(user.Id, roleIds);
                        _assignedUsers.Add(user);
                    }
                }
                else
                {
                    _assignedUsers.Add(user);
                }
            }

            if (usersToRemove.Any() || usersToAdd.Any())
            {
                ToastNotification.Success(L("Common.SavedSuccessfully"));
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Common.SaveFailed") + ": " + ex.Message);
        }
    }




    public class PermissionGroupViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<PermissionDefinitionViewModel> Permissions { get; set; } = [];
    }

    public class PermissionDefinitionViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public bool IsInherited { get; set; }
        public List<string> InheritedFrom { get; set; } = [];
    }

    public class RoleFormModel
    {
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; } = true;
        public string ConcurrencyStamp { get; set; } = string.Empty;
    }
}
