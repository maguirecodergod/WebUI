using System;
using System.Collections.Generic;

namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// Memory-resident reactive state tracking application Toast queue.
/// </summary>
public class ToastState
{
    private readonly List<ToastMessage> _toasts = new();

    /// <summary>
    /// Event triggered when Toasts are pushed or evicted.
    /// UI binding target for ToastContainer.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Thread-safe read-only snapshot list of active toast instances.
    /// </summary>
    public IReadOnlyList<ToastMessage> Toasts => _toasts.AsReadOnly();

    public void AddToast(ToastMessage toast, int maxToasts)
    {
        _toasts.Add(toast);

        // Discard oldest memory traces bridging max threshold organically
        if (_toasts.Count > maxToasts)
        {
            _toasts.RemoveAt(0);
        }

        NotifyStateChanged();
    }

    public void RemoveToast(Guid toastId)
    {
        var item = _toasts.Find(x => x.Id == toastId);
        if (item != null)
        {
            _toasts.Remove(item);
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
