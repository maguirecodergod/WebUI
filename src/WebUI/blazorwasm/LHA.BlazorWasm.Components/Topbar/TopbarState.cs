using System.Collections.ObjectModel;
using LHA.BlazorWasm.Components.Breadcrumb;

namespace LHA.BlazorWasm.Components.Topbar;

public class TopbarState
{
    public bool IsLoading { get; set; } = true;
    public bool IsSidebarCollapsed { get; set; }
    
    private List<NotificationModel> _notifications = new();
    public IReadOnlyList<NotificationModel> Notifications => _notifications.AsReadOnly();
    public int UnreadCount => _notifications.Count(n => !n.IsRead);

    public UserInfoModel? User { get; set; }
    
    public List<BreadcrumbItemModel> Breadcrumbs { get; set; } = new();
    
    public List<TopbarItemModel> DynamicItems { get; set; } = new();

    public event Action? OnStateChanged;

    public void NotifyStateChanged() => OnStateChanged?.Invoke();

    internal void UpdateNotifications(List<NotificationModel> notifications)
    {
        _notifications = notifications;
        NotifyStateChanged();
    }

    internal void AddNotification(NotificationModel notification)
    {
        _notifications.Insert(0, notification);
        NotifyStateChanged();
    }

    internal void SetSidebarState(bool collapsed)
    {
        IsSidebarCollapsed = collapsed;
        NotifyStateChanged();
    }

    internal void SetUser(UserInfoModel user)
    {
        User = user;
        NotifyStateChanged();
    }

    internal void SetBreadcrumbs(List<BreadcrumbItemModel> items)
    {
        Breadcrumbs = items;
        NotifyStateChanged();
    }
}
