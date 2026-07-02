namespace LHA.BlazorWasm.Services.Auth;

public sealed class SecurityRevocationState
{
    public bool IsVisible { get; private set; }
    public int RemainingSeconds { get; private set; }

    public event Action? OnChange;

    public void Show(int remainingSeconds)
    {
        IsVisible = true;
        RemainingSeconds = remainingSeconds;
        NotifyStateChanged();
    }

    public void UpdateRemainingSeconds(int remainingSeconds)
    {
        RemainingSeconds = remainingSeconds;
        NotifyStateChanged();
    }

    public void Hide()
    {
        IsVisible = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
