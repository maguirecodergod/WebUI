using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using LHA.BlazorWasm.Shared.Models.Localization;
using LHA.BlazorWasm.Services.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components;

/// <summary>
/// Base class for all LHA components to provide common internal injected services.
/// </summary>
public abstract class LhaComponentBase : ComponentBase
{
    [Inject] internal ILocalizationService Localizer { get; set; } = default!;
    [Inject] internal IToastService ToastNotification { get; set; } = default!;
    [Inject] internal IJSRuntime JS { get; set; } = default!;
    [Inject] internal NavigationManager Navigation { get; set; } = default!;
}
