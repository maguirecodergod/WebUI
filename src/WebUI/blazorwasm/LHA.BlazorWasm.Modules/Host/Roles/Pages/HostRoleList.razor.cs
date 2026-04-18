using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Table;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Roles;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Modules.Host.Roles.Components;

using LHA.BlazorWasm.HttpApi.Client.Clients;

namespace LHA.BlazorWasm.Modules.Host.Roles.Pages;

public partial class HostRoleList : LhaComponentBase
{
    [Inject] private RoleApiClient RoleAppService { get; set; } = default!;

    private List<IdentityRoleDto> _roles = [];
    private long _totalCount;
    private DataTable<IdentityRoleDto>? _dataTable;
    private bool _isDeleteDialogVisible;
    private IdentityRoleDto? _itemToDelete;
    private HostRoleCreateEditModal _createEditModal = default!;

    private GetIdentityRolesInput _input = new()
    {
        PageSize = 10,
        SorterKey = nameof(IdentityRoleDto.CreationTime),
        SorterIsAsc = false
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadRolesAsync();
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            var result = await RoleAppService.GetListAsync(_input);
            if (result != null)
            {
                _roles = result.Items.ToList();
                _totalCount = result.TotalCount;
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Roles.LoadError") + ": " + ex.Message);
        }
    }

    private async Task<DataTableResponse<IdentityRoleDto>> GetDataTableResponseAsync(DataTableRequest request)
    {
        _input.PageNumber = request.PageNumber;
        _input.PageSize = request.PageSize;
        _input.Filter = string.IsNullOrEmpty(request.SearchTerm) ? null : request.SearchTerm;

        if (request.Sorts is { Count: > 0 })
        {
            var sort = request.Sorts[0];
            _input.SorterKey = sort.Field;
            _input.SorterIsAsc = sort.Direction == SortDirection.Ascending;
        }
        else
        {
            _input.SorterKey = nameof(IdentityRoleDto.CreationTime);
            _input.SorterIsAsc = false;
        }

        await LoadRolesAsync();
        return new DataTableResponse<IdentityRoleDto> { Items = _roles, TotalCount = (int)_totalCount };
    }

    private void ShowSingleDeleteDialog(IdentityRoleDto role)
    {
        _itemToDelete = role;
        _isDeleteDialogVisible = true;
    }

    public async Task ExecuteDeleteAsync()
    {
        try
        {
            if (_itemToDelete != null)
            {
                await RoleAppService.DeleteAsync(_itemToDelete.Id);
                ToastNotification.Success(L("Common.DeletedSuccessfully"));
                _itemToDelete = null;
            }

            _isDeleteDialogVisible = false;
            await LoadRolesAsync();
            if (_dataTable != null) 
            {
                await _dataTable.RefreshAsync();
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            ToastNotification.Error(L("Common.DeleteFailed") + ": " + ex.Message);
        }
        finally
        {
            _isDeleteDialogVisible = false;
            StateHasChanged();
        }
    }

    private async Task OnRoleSavedAsync()
    {
        await LoadRolesAsync();
        if(_dataTable != null) await _dataTable.RefreshAsync();
    }
}
