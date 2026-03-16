namespace LHA.BlazorWasm.UI.Services;

/// <summary>
/// In-app notification service (distinct from toast — these are persistent notifications).
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Get all notifications.
    /// </summary>
    IReadOnlyList<NotificationItem> GetNotifications();

    /// <summary>
    /// Get unread count.
    /// </summary>
    int UnreadCount { get; }

    /// <summary>
    /// Add a notification.
    /// </summary>
    void Add(NotificationItem notification);

    /// <summary>
    /// Mark a notification as read.
    /// </summary>
    void MarkAsRead(Guid notificationId);

    /// <summary>
    /// Mark all notifications as read.
    /// </summary>
    void MarkAllAsRead();

    /// <summary>
    /// Remove a notification.
    /// </summary>
    void Remove(Guid notificationId);

    /// <summary>
    /// Clear all notifications.
    /// </summary>
    void Clear();

    /// <summary>
    /// Event raised when notifications change.
    /// </summary>
    event Action? OnNotificationsChanged;
}

/// <summary>
/// Represents an in-app notification.
/// </summary>
public sealed class NotificationItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string? Message { get; init; }
    public string? Icon { get; init; }
    public string? ActionUrl { get; init; }
    public CNotificationLevel Level { get; init; } = CNotificationLevel.Info;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}

public enum CNotificationLevel
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// Default implementation of the notification service.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly List<NotificationItem> _notifications = [];
    private const int MaxNotifications = 100;

    public event Action? OnNotificationsChanged;

    public IReadOnlyList<NotificationItem> GetNotifications()
    {
        return _notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList()
            .AsReadOnly();
    }

    public int UnreadCount => _notifications.Count(n => !n.IsRead);

    public void Add(NotificationItem notification)
    {
        _notifications.Insert(0, notification);

        // Cap at max
        while (_notifications.Count > MaxNotifications)
        {
            _notifications.RemoveAt(_notifications.Count - 1);
        }

        OnNotificationsChanged?.Invoke();
    }

    public void MarkAsRead(Guid notificationId)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
        if (notification is not null)
        {
            notification.IsRead = true;
            OnNotificationsChanged?.Invoke();
        }
    }

    public void MarkAllAsRead()
    {
        foreach (var notification in _notifications)
        {
            notification.IsRead = true;
        }
        OnNotificationsChanged?.Invoke();
    }

    public void Remove(Guid notificationId)
    {
        _notifications.RemoveAll(n => n.Id == notificationId);
        OnNotificationsChanged?.Invoke();
    }

    public void Clear()
    {
        _notifications.Clear();
        OnNotificationsChanged?.Invoke();
    }
}
