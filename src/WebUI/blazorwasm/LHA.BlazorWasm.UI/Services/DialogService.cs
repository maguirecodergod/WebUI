namespace LHA.BlazorWasm.UI.Services;

/// <summary>
/// Service for showing modal dialogs.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Show a confirmation dialog.
    /// </summary>
    Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmText = "Confirm", string cancelText = "Cancel");

    /// <summary>
    /// Show an alert dialog.
    /// </summary>
    Task AlertAsync(string message, string title = "Alert");

    /// <summary>
    /// Show a custom component in a dialog.
    /// </summary>
    Task<TResult?> ShowAsync<TResult>(Type componentType, string title, Dictionary<string, object>? parameters = null);

    /// <summary>
    /// Event raised when a dialog should be shown.
    /// </summary>
    event Action<DialogRequest>? OnDialogRequested;

    /// <summary>
    /// Event raised when the active dialog should be closed.
    /// </summary>
    event Action? OnDialogClosed;
}

/// <summary>
/// Dialog request model.
/// </summary>
public sealed class DialogRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Message { get; init; }
    public string ConfirmText { get; init; } = "Confirm";
    public string CancelText { get; init; } = "Cancel";
    public bool ShowCancel { get; init; } = true;
    public Type? ComponentType { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
    public TaskCompletionSource<bool>? ResultSource { get; init; }
}

/// <summary>
/// Default implementation of the dialog service.
/// </summary>
public sealed class DialogService : IDialogService
{
    public event Action<DialogRequest>? OnDialogRequested;
    public event Action? OnDialogClosed;

    public Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmText = "Confirm", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();

        OnDialogRequested?.Invoke(new DialogRequest
        {
            Title = title,
            Message = message,
            ConfirmText = confirmText,
            CancelText = cancelText,
            ShowCancel = true,
            ResultSource = tcs
        });

        return tcs.Task;
    }

    public Task AlertAsync(string message, string title = "Alert")
    {
        var tcs = new TaskCompletionSource<bool>();

        OnDialogRequested?.Invoke(new DialogRequest
        {
            Title = title,
            Message = message,
            ConfirmText = "OK",
            ShowCancel = false,
            ResultSource = tcs
        });

        return tcs.Task;
    }

    public Task<TResult?> ShowAsync<TResult>(Type componentType, string title, Dictionary<string, object>? parameters = null)
    {
        var tcs = new TaskCompletionSource<bool>();

        OnDialogRequested?.Invoke(new DialogRequest
        {
            Title = title,
            ComponentType = componentType,
            Parameters = parameters,
            ResultSource = tcs
        });

        return tcs.Task.ContinueWith(_ => default(TResult));
    }

    public void Close(bool result = false)
    {
        OnDialogClosed?.Invoke();
    }
}
