using LHA.BlazorWasm.Components.Breadcrumb;

namespace LHA.BlazorWasm.Components.Topbar;

public interface ITopbarService
{
    TopbarState State { get; }
    
    void ToggleSidebar();
    void SetSidebarCollapsed(bool collapsed);
    void SetLoading(bool isLoading);
    
    void AddNotification(NotificationModel notification);
    void SetNotifications(IEnumerable<NotificationModel> notifications);
    void MarkAsRead(string notificationId);
    void MarkAllAsRead();
    
    void SetUser(UserInfoModel user);
    void SetBreadcrumbs(IEnumerable<BreadcrumbItemModel> items);
    
    void RegisterDynamicItem(TopbarItemModel item);
    void UnregisterDynamicItem(string itemId);

    Task LoadUserProfileAsync(bool forceRefresh = false);
    Task LogoutAsync();
}
