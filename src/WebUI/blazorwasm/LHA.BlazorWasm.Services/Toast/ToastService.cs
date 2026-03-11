using System;
using System.Threading;
using System.Threading.Tasks;

namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// Service managing real-time pub/sub interactions pushing Toast UI components utilizing memory queues. 
/// Efficient polling timer auto-dismissal mechanics included.
/// </summary>
public class ToastService : IToastService
{
    private const int MaxVisibleToasts = 5;

    public ToastState State { get; } = new();

    public void Success(string message) => Show(message, ToastLevel.Success);
    public void Info(string message) => Show(message, ToastLevel.Info);
    public void Warning(string message) => Show(message, ToastLevel.Warning);
    public void Error(string message) => Show(message, ToastLevel.Error, 5000); // Give errors longer readability duration

    public void Show(string message, ToastLevel level, int duration = 3000)
    {
        var toast = new ToastMessage
        {
            Message = message,
            Level = level,
            Duration = duration
        };

        State.AddToast(toast, MaxVisibleToasts);

        // Instantiate encapsulated disconnected fire-and-forget timer logic cleanly bound off the UI thread
        if (duration > 0)
        {
            _ = RemoveAfterDelayAsync(toast.Id, duration);
        }
    }

    public void Remove(Guid toastId)
    {
        State.RemoveToast(toastId);
    }

    private async Task RemoveAfterDelayAsync(Guid toastId, int delay)
    {
        try
        {
            await Task.Delay(delay);
            Remove(toastId);
        }
        catch (Exception)
        {
            // Specifically sink cancellation or thread destruction errors safely
        }
    }
}
