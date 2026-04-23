using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.Shared.Contracts.Identity.Users;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Roles;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Modules.Tenant.Users.Pages;

public partial class TenantUserEdit : LHAComponentBase
{
    [Parameter] public Guid Id { get; set; }

    [Inject] private UserApiClient UserAppService { get; set; } = default!;
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;

    private IdentityUserDto? _user;
    private List<IdentityRoleDto> _availableRoles = [];
    private List<Guid> _selectedRoleIds = [];
    private bool _isSaving;
    private bool _isLoading = true;

    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems =
        [
            new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
            new() { Text = L("Menu.Tenant.Users"), Href = "/tenant/users", Icon = "bi bi-people" },
            new() { Text = L("Common.Edit"), Icon = "bi bi-pencil-square" }
        ];

        await Task.WhenAll(LoadUserAsync(), LoadRolesAsync());
        _isLoading = false;
    }

    private async Task LoadUserAsync()
    {
        try
        {
            _user = await UserAppService.GetAsync(Id);
            if (_user != null)
            {
                var userRoles = await UserAppService.GetRolesAsync(Id);
                _selectedRoleIds = userRoles?.Select(r => r.Id).ToList() ?? [];
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            var result = await RoleAppService.GetListAsync(new GetIdentityRolesInput { PageSize = 100 });
            if (result != null)
            {
                _availableRoles = result.Items.ToList();
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
    }

    private async Task HandleSubmitAsync()
    {
        if (_user == null) return;

        _isSaving = true;
        try
        {
            var input = new UpdateIdentityUserInput
            {
                UserName = _user.UserName,
                Email = _user.Email,
                Name = _user.Name,
                Surname = _user.Surname,
                PhoneNumber = _user.PhoneNumber,
                LockoutEnabled = _user.LockoutEnabled,
                ConcurrencyStamp = _user.ConcurrencyStamp,
                RoleIds = _selectedRoleIds
            };

            var result = await UserAppService.UpdateAsync(Id, input);
            if (result != null)
            {
                ToastNotification.Success(L("Common.UpdateSuccess"));
                Navigation.NavigateTo("/tenant/users");
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.UpdateFailed")}: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private void Cancel() => Navigation.NavigateTo("/tenant/users");
}
