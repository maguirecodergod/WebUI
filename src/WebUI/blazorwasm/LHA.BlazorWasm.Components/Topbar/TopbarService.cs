using LHA.BlazorWasm.Components.Breadcrumb;
using LHA.BlazorWasm.Services.Toast;

namespace LHA.BlazorWasm.Components.Topbar;

public class TopbarService : ITopbarService
{
    private readonly IToastService _toastService;
    public TopbarState State { get; } = new();

    public TopbarService(IToastService toastService)
    {
        _toastService = toastService;
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

    private CToastLevel MapSeverity(NotificationSeverity severity) => severity switch
    {
        NotificationSeverity.Success => CToastLevel.Success,
        NotificationSeverity.Warning => CToastLevel.Warning,
        NotificationSeverity.Error => CToastLevel.Error,
        _ => CToastLevel.Info
    };
}
