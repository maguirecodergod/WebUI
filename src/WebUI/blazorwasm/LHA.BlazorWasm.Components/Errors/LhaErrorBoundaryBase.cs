using LHA.BlazorWasm.Services.Toast;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Errors;

/// <summary>
/// Base class for ErrorBoundary components to provide common internal injected services.
/// </summary>
public abstract class LhaErrorBoundaryBase : ErrorBoundary
{
    [Inject] internal ILocalizationService Localizer { get; set; } = default!;
    [Inject] internal IToastService ToastNotification { get; set; } = default!;
    [Inject] internal IJSRuntime JS { get; set; } = default!;
    [Inject] internal NavigationManager Navigation { get; set; } = default!;
}
