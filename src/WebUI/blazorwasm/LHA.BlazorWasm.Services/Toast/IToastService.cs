namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// Abstraction dictating API rules for publishing enterprise Toast Notifications.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Singleton reactive state containing DOM queues.
    /// </summary>
    ToastState State { get; }

    /// <summary>
    /// Dispatches a high-priority green 'Success' notification.
    /// </summary>
    void Success(string message, string? title = null);

    /// <summary>
    /// Dispatches a standard blue 'Info' notification.
    /// </summary>
    void Info(string message, string? title = null);

    /// <summary>
    /// Dispatches a yellow 'Warning' notification.
    /// </summary>
    void Warning(string message, string? title = null);

    /// <summary>
    /// Dispatches a critical red 'Error' notification.
    /// </summary>
    void Error(string message, string? title = null);

    /// <summary>
    /// Fully robust manual override configuration to dictate precise duration and formatting limits.
    /// </summary>
    void Show(string message, ToastLevel level, string? title = null, int duration = 3000);
    
    /// <summary>
    /// Programmatically triggers the UI removal via ToastId bypassing internal timer logic.
    /// </summary>
    void Remove(Guid toastId);
}
