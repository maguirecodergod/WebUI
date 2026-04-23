using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.Shared.Contracts.Identity.Users;
using LHA.Shared.Contracts.Identity;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Table;

namespace LHA.BlazorWasm.Modules.Tenant.Users.Pages;

public partial class TenantUserList : LHAComponentBase
{
    [Inject] private UserApiClient UserAppService { get; set; } = default!;

    private List<IdentityUserDto> _users = [];
    private GetIdentityUsersInput _input = new() { PageSize = 20, PageNumber = 1 };
    private long _totalCount;
    private bool _isLoading;

    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems = [];

    protected override void OnInitialized()
    {
        _breadcrumbItems =
        [
            new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
            new() { Text = L("Menu.Tenant.Users"), Icon = "bi bi-people" }
        ];
    }

    private async Task<DataTableResponse<IdentityUserDto>> GetDataTableResponseAsync(DataTableRequest request)
    {
        _input.PageNumber = request.PageNumber;
        _input.PageSize = request.PageSize;
        _input.Filter = request.SearchTerm;

        await LoadUsersAsync();

        return new DataTableResponse<IdentityUserDto>
        {
            Items = _users,
            TotalCount = (int)_totalCount
        };
    }

    private async Task LoadUsersAsync()
    {
        _isLoading = true;
        try
        {
            var result = await UserAppService.GetListAsync(_input);
            if (result != null)
            {
                _users = result.Items.ToList();
                _totalCount = result.TotalCount;
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void NavigateToCreate() => Navigation.NavigateTo("/tenant/users/create");

    private void NavigateToEdit(Guid id) => Navigation.NavigateTo($"/tenant/users/{id}");

    private async Task DeleteUserAsync(IdentityUserDto user)
    {
        if (await JS.InvokeAsync<bool>("confirm", new object[] { L("Users.DeleteConfirmation", new object[] { user.UserName }) }))
        {
            try
            {
                await UserAppService.DeleteAsync(user.Id);
                ToastNotification.Success(L("Common.DeleteSuccess"));
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"{L("Common.DeleteFailed")}: {ex.Message}");
            }
        }
    }
}
