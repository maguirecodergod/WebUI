namespace LHA.BlazorWasm.UI.State;

/// <summary>
/// Centralized UI state store.
/// Manages all reactive UI states and provides a unified subscription mechanism.
/// </summary>
public sealed class UiStateStore
{
    // ── Sidebar ───────────────────────────────────────────────────────
    public bool IsSidebarExpanded { get; private set; } = true;
    public bool IsSidebarMini { get; private set; }
    public bool IsMobileDrawerOpen { get; private set; }

    // ── Command Palette ───────────────────────────────────────────────
    public bool IsCommandPaletteOpen { get; private set; }

    // ── Notifications ─────────────────────────────────────────────────
    public int UnreadNotificationCount { get; private set; }

    // ── Loading ───────────────────────────────────────────────────────
    public bool IsGlobalLoading { get; private set; }
    public string? LoadingMessage { get; private set; }

    // ── Events ────────────────────────────────────────────────────────
    public event Action? OnStateChanged;
    public event Action<string>? OnPropertyChanged;

    private void NotifyStateChanged(string propertyName = "")
    {
        OnPropertyChanged?.Invoke(propertyName);
        OnStateChanged?.Invoke();
    }

    // ── Sidebar Actions ───────────────────────────────────────────────
    public void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
        IsSidebarMini = !IsSidebarExpanded;
        NotifyStateChanged(nameof(IsSidebarExpanded));
    }

    public void SetSidebarExpanded(bool expanded)
    {
        IsSidebarExpanded = expanded;
        IsSidebarMini = !expanded;
        NotifyStateChanged(nameof(IsSidebarExpanded));
    }

    public void ToggleMobileDrawer()
    {
        IsMobileDrawerOpen = !IsMobileDrawerOpen;
        NotifyStateChanged(nameof(IsMobileDrawerOpen));
    }

    public void CloseMobileDrawer()
    {
        IsMobileDrawerOpen = false;
        NotifyStateChanged(nameof(IsMobileDrawerOpen));
    }

    // ── Command Palette Actions ───────────────────────────────────────
    public void OpenCommandPalette()
    {
        IsCommandPaletteOpen = true;
        NotifyStateChanged(nameof(IsCommandPaletteOpen));
    }

    public void CloseCommandPalette()
    {
        IsCommandPaletteOpen = false;
        NotifyStateChanged(nameof(IsCommandPaletteOpen));
    }

    public void ToggleCommandPalette()
    {
        IsCommandPaletteOpen = !IsCommandPaletteOpen;
        NotifyStateChanged(nameof(IsCommandPaletteOpen));
    }

    // ── Notification Actions ──────────────────────────────────────────
    public void SetUnreadCount(int count)
    {
        UnreadNotificationCount = count;
        NotifyStateChanged(nameof(UnreadNotificationCount));
    }

    public void IncrementUnreadCount()
    {
        UnreadNotificationCount++;
        NotifyStateChanged(nameof(UnreadNotificationCount));
    }

    // ── Loading Actions ───────────────────────────────────────────────
    public void SetLoading(bool loading, string? message = null)
    {
        IsGlobalLoading = loading;
        LoadingMessage = message;
        NotifyStateChanged(nameof(IsGlobalLoading));
    }
}
