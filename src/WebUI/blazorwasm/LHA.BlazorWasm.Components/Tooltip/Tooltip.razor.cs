using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Tooltip;

/// <summary>
/// Underlying logic code-behind for Tooltip.razor enforcing manual/timed delay rendering mechanisms
/// utilizing local state for pure C# driven component rendering without JavaScript interop dependencies.
/// </summary>
public partial class Tooltip : IDisposable
{
    [Parameter] public string? Text { get; set; }
    [Parameter] public CTooltipPlacement Placement { get; set; } = CTooltipPlacement.Top;
    [Parameter] public CTooltipTrigger Trigger { get; set; } = CTooltipTrigger.Hover;
    [Parameter] public int Delay { get; set; } = 200;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    /// <summary>
    /// Support for rich Blazor custom nested component templates.
    /// </summary>
    [Parameter] public RenderFragment? TooltipContent { get; set; }

    private bool _isVisible;
    private readonly string _tooltipId = $"tooltip-{Guid.NewGuid():N}";
    private CancellationTokenSource? _delayCts;


    private IJSObjectReference? _jsModule;
    private ElementReference _triggerRef;
    private ElementReference _tooltipRef;

    private string WrapperClass => $"tooltip-wrapper {Class}".Trim();
    private string TooltipClass => $"tooltip tooltip-{Placement.ToString().ToLowerInvariant()}";

    /// <summary>
    /// Manually requests the tooltip to appear optionally overriding the defined UI trigger limits.
    /// Respects disabled states. Supports parameterized thread timing delays.
    /// </summary>
    public async Task ShowAsync()
    {
        if (_isVisible || Disabled) return;

        _delayCts?.Cancel();
        _delayCts = new CancellationTokenSource();

        if (Delay > 0)
        {
            try
            {
                await Task.Delay(Delay, _delayCts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }

        _isVisible = true;
        StateHasChanged();

        if (_jsModule != null)
        {
            // Call JS internally asynchronously without blocking UI render cycle
            _ = _jsModule.InvokeVoidAsync("showTooltip", _triggerRef, _tooltipRef, new { placement = Placement.ToString().ToLowerInvariant() });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/LHA.BlazorWasm.Components/Tooltip/Tooltip.razor.js");
        }
    }

    /// <summary>
    /// Forcibly hides the tooltip dismissing it immediately regardless of trigger types.
    /// </summary>
    public void Hide()
    {
        _delayCts?.Cancel();
        if (_isVisible)
        {
            _isVisible = false;
            if (_jsModule != null)
            {
                _ = _jsModule.InvokeVoidAsync("hideTooltip", _tooltipRef);
            }
            StateHasChanged();
        }
    }

    private async Task OnMouseEnter()
    {
        if (Trigger == CTooltipTrigger.Hover)
        {
            await ShowAsync();
        }
    }

    private void OnMouseLeave()
    {
        if (Trigger == CTooltipTrigger.Hover)
        {
            Hide();
        }
    }

    private async Task OnFocusIn()
    {
        if (Trigger == CTooltipTrigger.Focus)
        {
            await ShowAsync();
        }
    }

    private void OnFocusOut()
    {
        if (Trigger == CTooltipTrigger.Focus)
        {
            Hide();
        }
    }

    private async Task OnClick()
    {
        if (Trigger == CTooltipTrigger.Click)
        {
            if (_isVisible)
            {
                Hide();
            }
            else
            {
                await ShowAsync();
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _delayCts?.Cancel();
        _delayCts?.Dispose();
        if (_jsModule != null)
        {
            _ = _jsModule.InvokeVoidAsync("dispose", _tooltipRef);
        }
    }
}
