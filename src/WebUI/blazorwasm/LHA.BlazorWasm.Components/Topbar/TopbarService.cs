using LHA.BlazorWasm.Components.Breadcrumb;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.BlazorWasm.Services.Auth;
using LHA.BlazorWasm.Services.Toast;
using Microsoft.AspNetCore.Components.Authorization;

namespace LHA.BlazorWasm.Components.Topbar;

public class TopbarService : ITopbarService
{
    private readonly IToastService _toastService;
    private readonly AuthApiClient _authApiClient;
    private readonly ApiAuthenticationStateProvider _authStateProvider;
    private readonly IPermissionService _permissionService;

    public TopbarState State { get; } = new();

    public TopbarService(
        IToastService toastService,
        AuthApiClient authApiClient,
        AuthenticationStateProvider authStateProvider,
        IPermissionService permissionService)
    {
        _toastService = toastService;
        _authApiClient = authApiClient;
        _authStateProvider = (ApiAuthenticationStateProvider)authStateProvider;
        _permissionService = permissionService;
    }

    public void ToggleSidebar()
    {
        State.SetSidebarState(!State.IsSidebarCollapsed);
    }

    public void SetSidebarCollapsed(bool collapsed)
    {
        State.SetSidebarState(collapsed);
    }
    public void SetLoading(bool isLoading)
    {
        State.IsLoading = isLoading;
        State.NotifyStateChanged();
    }

    public void AddNotification(NotificationModel notification)
    {
        State.AddNotification(notification);
        
        // Integrate with Toast system
        _toastService.Show(notification.Message, MapSeverity(notification.Severity), notification.Title);
    }

    public void SetNotifications(IEnumerable<NotificationModel> notifications)
    {
        State.UpdateNotifications(notifications.ToList());
    }

    public void MarkAsRead(string notificationId)
    {
        var notification = State.Notifications.FirstOrDefault(n => n.Id == notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            State.NotifyStateChanged();
        }
    }

    public void MarkAllAsRead()
    {
        foreach (var notification in State.Notifications)
        {
            notification.IsRead = true;
        }
        State.NotifyStateChanged();
    }

    public void SetUser(UserInfoModel user)
    {
        State.SetUser(user);
    }

    public void SetBreadcrumbs(IEnumerable<BreadcrumbItemModel> items)
    {
        State.SetBreadcrumbs(items.ToList());
    }

    public void RegisterDynamicItem(TopbarItemModel item)
    {
        State.DynamicItems.Add(item);
        State.DynamicItems = State.DynamicItems.OrderBy(i => i.Order).ToList();
        State.NotifyStateChanged();
    }

    public void UnregisterDynamicItem(string itemId)
    {
        State.DynamicItems.RemoveAll(i => i.Id == itemId);
        State.NotifyStateChanged();
    }

    public async Task LoadUserProfileAsync()
    {
        try
        {
            SetLoading(true);
            var currentUser = await _authApiClient.GetCurrentUserAsync();
            if (currentUser != null)
            {
                SetUser(new UserInfoModel
                {
                    DisplayName = currentUser.Name ?? currentUser.UserName,
                    Email = currentUser.Email,
                    Role = currentUser.Roles.FirstOrDefault() // Just take the first one for now
                });

                _permissionService.SetPermissions(currentUser.Permissions);
            }
        }
        finally
        {
            SetLoading(false);
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOutAsync();
        _toastService.Show("Logged out successfully", CToastLevel.Info);
    }

    private CToastLevel MapSeverity(NotificationSeverity severity) => severity switch
    {
        NotificationSeverity.Success => CToastLevel.Success,
        NotificationSeverity.Warning => CToastLevel.Warning,
        NotificationSeverity.Error => CToastLevel.Error,
        _ => CToastLevel.Info
    };
}
