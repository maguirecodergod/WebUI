using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Topbar;

public partial class NotificationBell : LHAComponentBase
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
        NotificationSeverity.Success => """<i class="bi bi-check-circle"></i>""",
        NotificationSeverity.Warning => """<i class="bi bi-exclamation-triangle"></i>""",
        NotificationSeverity.Error => """<i class="bi bi-x-circle"></i>""",
        _ => """<i class="bi bi-info-circle"></i>"""
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
