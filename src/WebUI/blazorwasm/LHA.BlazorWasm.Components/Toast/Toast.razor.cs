using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Toast;

namespace LHA.BlazorWasm.Components.Toast;

/// <summary>
/// Core code-behind binding rendering loops and CSS animation timings natively.
/// </summary>
public partial class Toast : IDisposable
{
    [Parameter, EditorRequired] public ToastMessage Message { get; set; } = default!;

    private bool _isClosing;

    protected override void OnInitialized()
    {
        // Triggered upon insertion. Native UI rendering picks up immediate 'entering' state implicitly via DOM DOM
    }

    private string GetToastClass()
    {
        return Message?.Level switch
        {
            ToastLevel.Success => "toast-success",
            ToastLevel.Info => "toast-info",
            ToastLevel.Warning => "toast-warning",
            ToastLevel.Error => "toast-error",
            _ => "toast-info"
        };
    }

    private string GetAnimationClass()
    {
        return _isClosing ? "toast-exit" : "toast-enter";
    }

    private async Task CloseToast()
    {
        if (_isClosing) return;

        _isClosing = true;

        // Push DOM update requesting outgoing slide animation
        StateHasChanged();

        // Wait strictly for the CSS transition length before killing object entirely via Service
        await Task.Delay(400); // Maps 1:1 strictly to the 0.4s CSS ruleset

        if (Message != null)
        {
            ToastNotification.Remove(Message.Id);
        }
    }

    public void Dispose()
    {
        // GC Cleanup
    }
}
