using System;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Toast;

namespace LHA.BlazorWasm.Components.Toast;

/// <summary>
/// Root-level Application DOM host anchoring absolute notifications.
/// Re-renders iteratively via strict implicit service Event bindings.
/// </summary>
public partial class ToastContainer : IDisposable
{
    [Inject] private IToastService ToastService { get; set; } = default!;

    protected override void OnInitialized()
    {
        ToastService.State.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        ToastService.State.OnChange -= StateHasChanged;
    }
}
