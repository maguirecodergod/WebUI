using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.Shared.Contracts.Identity.Users;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Roles;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Modules.Tenant.Users.Pages;

public partial class TenantUserCreate : LHAComponentBase
{
    [Inject] private UserApiClient UserAppService { get; set; } = default!;
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;

    private CreateIdentityUserInput _model = new() 
    { 
        UserName = string.Empty, 
        Email = string.Empty, 
        Password = string.Empty 
    };
    
    private List<IdentityRoleDto> _availableRoles = [];
    private List<Guid> _selectedRoleIds = [];
    private bool _isSaving;
    private string? _passwordConfirmation;

    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems =
        [
            new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
            new() { Text = L("Menu.Tenant.Users"), Href = "/tenant/users", Icon = "bi bi-people" },
            new() { Text = L("Common.Create"), Icon = "bi bi-plus-circle" }
        ];

        await LoadRolesAsync();
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
        if (_model.Password != _passwordConfirmation)
        {
            ToastNotification.Error(L("Users.PasswordMismatch"));
            return;
        }

        _isSaving = true;
        try
        {
            // Update model with selected roles
            var finalModel = new CreateIdentityUserInput
            {
                UserName = _model.UserName,
                Email = _model.Email,
                Password = _model.Password,
                Name = _model.Name,
                Surname = _model.Surname,
                PhoneNumber = _model.PhoneNumber,
                RoleIds = _selectedRoleIds
            };

            var result = await UserAppService.CreateAsync(finalModel);
            if (result != null)
            {
                ToastNotification.Success(L("Common.CreateSuccess"));
                Navigation.NavigateTo("/tenant/users");
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.CreateFailed")}: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }

    private void Cancel() => Navigation.NavigateTo("/tenant/users");
}
