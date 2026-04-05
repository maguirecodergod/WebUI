using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Topbar;

public enum TopbarActionType
{
    Link,
    Button,
    Toggle,
    Dropdown,
    Custom
}

public class TopbarItemModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Icon { get; set; }
    public TopbarActionType ActionType { get; set; } = TopbarActionType.Link;
    public RenderFragment? CustomContent { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? Permission { get; set; }
    public Action? OnClick { get; set; }
    public int Order { get; set; }
}

public class NotificationModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; }
    public string? Icon { get; set; }
    public string? Link { get; set; }
    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;
}

public enum NotificationSeverity
{
    Info,
    Success,
    Warning,
    Error
}

public class SearchResultItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public string? Url { get; set; }
    public Action? OnSelect { get; set; }
    public string? Shortcut { get; set; }
    public bool IsActive { get; set; }
}

public class UserInfoModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Role { get; set; }
}
