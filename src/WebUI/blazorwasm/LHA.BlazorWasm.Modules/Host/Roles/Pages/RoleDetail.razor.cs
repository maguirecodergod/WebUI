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
using LHA.BlazorWasm.Components;

namespace LHA.BlazorWasm.Modules.Host.Roles.Pages;

public partial class RoleDetail : LHAComponentBase
{
    [Parameter] public Guid Id { get; set; }
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;
    [Inject] private UserApiClient UserAppService { get; set; } = default!;
    [Inject] private PermissionGroupApiClient PermissionGroupAppService { get; set; } = default!;
    [Inject] private PermissionApiClient PermissionAppService { get; set; } = default!;

    private RoleEditViewModel _roleModel = new();
    private Guid? _roleTenantId;
    private List<PermissionGroupViewModel> _groups = [];
    private List<IdentityUserDto> _assignedUsers = [];
    private List<PermissionGroupViewModel> _userGroups = [];
    private IdentityUserDto? _selectedUser;
    private string _permissionFilter = string.Empty;
    /// <summary>Zero-based index tracking which tab is active (bound to Tabs @bind-ActiveIndex).</summary>
    private int _activeTabIndex;
    private bool _isSaving;
    private bool _isUserPermissionsLoading;
    private bool _isSavingUserPermissions;
    private bool _isPermissionsDialogVisible;
    private bool _hasRoleChanges;
    private bool _hasUserPermissionChanges;
    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems = new List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel>
        {
            new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
            new() { Text = L("Menu.Host.Roles"), Href = "/host/roles", Icon = "bi bi-shield-lock" },
            new() { Text = L("Common.Detail"), Icon = "bi bi-info-circle" }
        };

        await LoadRoleAsync();
        await LoadPermissionDefinitionsAsync();
        await LoadRolePermissionsAsync();
        await LoadAssignedUsersAsync();
    }

    private async Task LoadRoleAsync()
    {
        var role = await RoleAppService.GetAsync(Id);
        if (role != null)
        {
            _roleTenantId = role.TenantId;
            _roleModel = new RoleEditViewModel
            {
                Name = role.Name,
                IsDefault = role.IsDefault,
                IsPublic = role.IsPublic,
                IsStatic = role.IsStatic,
                ConcurrencyStamp = role.ConcurrencyStamp
            };
        }
    }

    private async Task LoadPermissionDefinitionsAsync()
    {
        var groupsRes = await PermissionGroupAppService.GetListAsync(new GetPermissionGroupsInput { PageSize = 100 });
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

    private async Task LoadRolePermissionsAsync()
    {
        if (_groups.Count == 0) return;

        var permResult = await PermissionAppService.GetAsync(new GetPermissionListInput
        {
            ProviderName = PermissionGrantProviderName.Role,
            ProviderKey = Id.ToString().ToLowerInvariant()
        }, _roleTenantId);

        // Fallback to Name if ID returns 0 grants
        if (permResult == null || permResult.Count == 0)
        {
            permResult = await PermissionAppService.GetAsync(new GetPermissionListInput
            {
                ProviderName = PermissionGrantProviderName.Role,
                ProviderKey = _roleModel.Name
            }, _roleTenantId);
        }

        if (permResult != null && permResult.Count > 0)
        {
            var finalGrants = permResult.Any(x => x.IsGranted)
                ? permResult.Where(x => x.IsGranted).Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : permResult.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var group in _groups)
            {
                foreach (var p in group.Permissions)
                {
                    if (finalGrants.Contains(p.Name))
                    {
                        p.IsGranted = true;
                    }
                }
            }
        }
        else
        {
            // Template Fallback for system roles if no grants found
            var templates = await PermissionAppService.GetTemplatesAsync(new GetPermissionTemplatesInput { PageSize = 100 });
            var matchingTemplate = templates?.Items?.FirstOrDefault(t => string.Equals(t.Name, _roleModel.Name, StringComparison.OrdinalIgnoreCase));

            if (matchingTemplate != null)
            {
                var templatePerms = matchingTemplate.Groups.SelectMany(g => g.Permissions).Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var group in _groups)
                {
                    foreach (var p in group.Permissions)
                    {
                        if (templatePerms.Contains(p.Name))
                        {
                            p.IsGranted = true;
                            p.IsFromTemplate = true;
                        }
                    }
                }
            }
        }
        StateHasChanged();
    }

    private async Task LoadAssignedUsersAsync()
    {
        var usersRes = await UserAppService.GetListAsync(new GetIdentityUsersInput { RoleId = Id, PageSize = 1000 });
        _assignedUsers = usersRes?.Items?.ToList() ?? [];
    }

    private void GoBack() => Navigation.NavigateTo("/host/roles");

    /// <summary>Fired by the Tabs component when the user switches tabs.</summary>
    private Task OnTabChangedAsync(TabDefinition tab)
    {
        // Extend here if you need to react to specific tab activations,
        // e.g. lazy-loading data, updating breadcrumb etc.
        return Task.CompletedTask;
    }

    private async Task SaveRoleAsync()
    {
        _isSaving = true;
        try
        {
            // Update Role Info (Only for non-static roles, as static role names are fixed)
            if (!_roleModel.IsStatic)
            {
                await RoleAppService.UpdateAsync(Id, new UpdateIdentityRoleInput
                {
                    Name = _roleModel.Name,
                    IsDefault = _roleModel.IsDefault,
                    IsPublic = _roleModel.IsPublic,
                    ConcurrencyStamp = _roleModel.ConcurrencyStamp ?? string.Empty
                });
            }

            // Update Role Permissions
            var permissions = _groups
                .SelectMany(g => g.Permissions)
                .Select(p => new PermissionGrantInput { Name = p.Name, IsGranted = p.IsGranted })
                .ToList();

            await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
            {
                ProviderName = PermissionGrantProviderName.Role,
                ProviderKey = Id.ToString().ToLowerInvariant(),
                Permissions = permissions
            }, _roleTenantId);

            ToastNotification.Success("Lưu thay đổi thành công!");
            _hasRoleChanges = false;
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"Lỗi khi lưu: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task SelectUser(IdentityUserDto user)
    {
        _selectedUser = user;
        _isPermissionsDialogVisible = true;
        _isUserPermissionsLoading = true;
        _hasUserPermissionChanges = false;

        if (_userGroups.Count == 0 && _groups.Count > 0)
        {
            _userGroups = _groups.Select(g => new PermissionGroupViewModel
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

        StateHasChanged();
        await LoadUserEffectivePermissionsAsync(user);
        _isUserPermissionsLoading = false;
        StateHasChanged();
    }

    private async Task LoadUserEffectivePermissionsAsync(IdentityUserDto user)
    {
        // 1. Reset
        foreach (var group in _userGroups)
        {
            foreach (var p in group.Permissions)
            {
                p.IsGranted = false;
                p.IsInherited = false;
                p.IsDirectOverride = false;
                p.IsFromTemplate = false;
                p.InheritedFrom = [];
            }
        }

        // 2. Load Role-based grants (Inherited)
        var userRolesRes = await UserAppService.GetRolesAsync(user.Id);
        var roleGrants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var roleMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (userRolesRes != null)
        {
            foreach (var role in userRolesRes)
            {
                // Fetch permissions for this specific role
                var rolePerms = await PermissionAppService.GetAsync(new GetPermissionListInput
                {
                    ProviderName = PermissionGrantProviderName.Role,
                    ProviderKey = role.Id.ToString().ToLowerInvariant()
                }, user.TenantId);

                // Fallback to name for system roles
                if (rolePerms == null || rolePerms.Count == 0)
                {
                    rolePerms = await PermissionAppService.GetAsync(new GetPermissionListInput
                    {
                        ProviderName = PermissionGrantProviderName.Role,
                        ProviderKey = role.Name
                    }, user.TenantId);
                }

                if (rolePerms != null)
                {
                    var grantedInRole = rolePerms.Any(x => x.IsGranted)
                        ? rolePerms.Where(x => x.IsGranted).Select(x => x.Name)
                        : rolePerms.Select(x => x.Name);

                    foreach (var name in grantedInRole)
                    {
                        roleGrants.Add(name);
                        if (!roleMap.ContainsKey(name)) roleMap[name] = new List<string>();
                        if (!roleMap[name].Contains(role.Name)) roleMap[name].Add(role.Name);
                    }
                }
            }
        }

        // 3. Load Direct user overrides
        var userGrantsRes = await PermissionAppService.GetAsync(new GetPermissionListInput
        {
            ProviderName = PermissionGrantProviderName.User,
            ProviderKey = user.Id.ToString().ToLowerInvariant()
        }, user.TenantId);

        var userDirectGrants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var userDirectRevokes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (userGrantsRes != null)
        {
            foreach (var g in userGrantsRes)
            {
                if (g.IsGranted) userDirectGrants.Add(g.Name);
                else userDirectRevokes.Add(g.Name);
            }
        }

        // 4. Calculate Final Effective State
        foreach (var group in _userGroups)
        {
            foreach (var p in group.Permissions)
            {
                p.IsInherited = roleGrants.Contains(p.Name);
                p.InheritedFrom = roleMap.ContainsKey(p.Name) ? roleMap[p.Name] : [];

                if (userDirectGrants.Contains(p.Name))
                {
                    p.IsGranted = true;
                    p.IsDirectOverride = true;
                }
                else if (userDirectRevokes.Contains(p.Name))
                {
                    p.IsGranted = false;
                    p.IsDirectOverride = true;
                }
                else
                {
                    p.IsGranted = p.IsInherited;
                    p.IsDirectOverride = false;
                }
            }
        }
    }

    private void OnRolePermissionChanged(PermissionDefinitionViewModel p, bool granted)
    {
        p.IsGranted = granted;
        p.IsFromTemplate = false;
        _hasRoleChanges = true;
        StateHasChanged();
    }

    private void ToggleUserPermission(PermissionDefinitionViewModel p, bool granted)
    {
        if (_selectedUser == null) return;

        p.IsGranted = granted;
        p.IsDirectOverride = true; // Any manual toggle in this view is an override
        _hasUserPermissionChanges = true;
        StateHasChanged();
    }

    private async Task SaveUserPermissionsAsync()
    {
        if (_selectedUser == null) return;

        _isSavingUserPermissions = true;
        try
        {
            var permissions = _userGroups
                .SelectMany(g => g.Permissions)
                .Where(p => p.IsDirectOverride)
                .Select(p => new PermissionGrantInput { Name = p.Name, IsGranted = p.IsGranted })
                .ToList();

            await PermissionAppService.UpdateAsync(new UpdatePermissionsInput
            {
                ProviderName = PermissionGrantProviderName.User,
                ProviderKey = _selectedUser.Id.ToString().ToLowerInvariant(),
                Permissions = permissions
            }, _selectedUser.TenantId);

            ToastNotification.Success("Cập nhật quyền người dùng thành công!");
            _hasUserPermissionChanges = false;
            _isPermissionsDialogVisible = false;
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
        finally
        {
            _isSavingUserPermissions = false;
        }
    }

}
