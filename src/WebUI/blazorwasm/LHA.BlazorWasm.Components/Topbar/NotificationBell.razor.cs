using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Topbar;

public partial class NotificationBell : LhaComponentBase
{
    [Inject] public ITopbarService TopbarService { get; set; } = default!;
    
    [Parameter] public int Count { get; set; }

    private bool _isOpen;

    private void ToggleDropdown()
    {
        _isOpen = !_isOpen;
    }

    private void MarkAllAsRead()
    {
        TopbarService.MarkAllAsRead();
    }

    private void HandleClick(NotificationModel notification)
    {
        TopbarService.MarkAsRead(notification.Id);
        if (!string.IsNullOrEmpty(notification.Link))
        {
            Navigation.NavigateTo(notification.Link);
        }
        _isOpen = false;
    }

    private string DefaultIcon(NotificationSeverity severity) => severity switch
    {
        NotificationSeverity.Success => """<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"></polyline></svg>""",
        NotificationSeverity.Warning => """<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>""",
        NotificationSeverity.Error => """<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg>""",
        _ => """<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>"""
    };

    private string GetTimeAgo(DateTime dateTime)
    {
        var span = DateTime.Now - dateTime;
        if (span.TotalDays > 365) return $"{(int)(span.TotalDays / 365)}y ago";
        if (span.TotalDays > 30) return $"{(int)(span.TotalDays / 30)}mo ago";
        if (span.TotalDays > 7) return $"{(int)(span.TotalDays / 7)}w ago";
        if (span.TotalDays > 1) return $"{(int)span.TotalDays}d ago";
        if (span.TotalHours > 1) return $"{(int)span.TotalHours}h ago";
        if (span.TotalMinutes > 1) return $"{(int)span.TotalMinutes}m ago";
        return "Just now";
    }
}
