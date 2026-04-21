using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Dialog;

public partial class PremiumDialog : LhaComponentBase
{
    private ElementReference _overlayRef;
    private bool _shouldFocus = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocus)
        {
            _shouldFocus = false;
            try
            {
                await _overlayRef.FocusAsync();
            }
            catch { /* Ignore if focus fails */ }
        }
    }

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

    private bool _previousIsVisible;

    protected override void OnParametersSet()
    {
        if (IsVisible && !_previousIsVisible)
        {
            _shouldFocus = true;
        }
        _previousIsVisible = IsVisible;
    }

    [Parameter] public DialogSize Size { get; set; } = DialogSize.Medium;
    [Parameter] public DialogType Type { get; set; } = DialogType.Default;

    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }

    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowFooter { get; set; } = true;
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public bool CloseOnOverlayClick { get; set; } = false;

    [Parameter] public string OkText { get; set; } = "Common_Confirm";
    [Parameter] public string CancelText { get; set; } = "Common_Cancel";
    [Parameter] public bool ShowCancelButton { get; set; } = true;

    [Parameter] public EventCallback OnOk { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    protected bool IsClosing { get; set; } = false;

    public async Task OpenAsync()
    {
        IsVisible = true;
        await IsVisibleChanged.InvokeAsync(IsVisible);
        StateHasChanged();
    }

    public async Task CloseAsync()
    {
        IsClosing = true;
        StateHasChanged();

        await Task.Delay(300); // Wait for transition

        IsClosing = false;
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(IsVisible);
        StateHasChanged();
    }

    protected async Task HandleOkAsync()
    {
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
        else
        {
            await CloseAsync();
        }
    }

    protected async Task ExtendedOkAsync(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        await HandleOkAsync();
    }

    protected async Task HandleCancelAsync()
    {
        if (OnCancel.HasDelegate)
        {
            await OnCancel.InvokeAsync();
        }
        else
        {
            await CloseAsync();
        }
    }

    protected async Task ExtendedCancelAsync(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        await HandleCancelAsync();
    }

    protected async Task HandleOverlayClick()
    {
        if (CloseOnOverlayClick && !IsClosing)
        {
            await CloseAsync();
        }
    }

    protected async Task HandleKeyDown(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && !IsClosing)
        {
            await HandleCancelAsync();
        }
    }

    private string GetSizeClass()
    {
        return Size switch
        {
            DialogSize.Small => "dialog-sm",
            DialogSize.Medium => "dialog-md",
            DialogSize.Large => "dialog-lg",
            DialogSize.ExtraLarge => "dialog-xl",
            DialogSize.FullScreen => "dialog-fullscreen",
            _ => "dialog-md"
        };
    }

    private string GetTypeClass()
    {
        return Type switch
        {
            DialogType.Confirmation => "dialog-confirmation",
            DialogType.Warning => "dialog-warning",
            DialogType.Error => "dialog-error",
            DialogType.Success => "dialog-success",
            DialogType.Information => "dialog-info",
            _ => "dialog-default"
        };
    }
}
