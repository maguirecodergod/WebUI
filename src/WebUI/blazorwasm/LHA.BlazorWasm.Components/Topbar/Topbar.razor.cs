using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace LHA.BlazorWasm.Components.Topbar;

public partial class Topbar : LhaComponentBase, IDisposable
{
    [Inject] public ITopbarService TopbarService { get; set; } = default!;

    [Parameter] public RenderFragment? LeftContent { get; set; }
    [Parameter] public RenderFragment? CenterContent { get; set; }
    [Parameter] public RenderFragment? RightContent { get; set; }

    [Parameter] public string AppName { get; set; } = "LHA WebUI";
    [Parameter] public string? LogoSvg { get; set; }

    [Parameter] public EventCallback OnSidebarToggle { get; set; }

    private string _searchText = string.Empty;
    private bool _isNotificationOpen;
    private bool _isProfileOpen;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        TopbarService.State.OnStateChanged += StateHasChanged;
    }

    private async Task HandleSidebarToggle()
    {
        TopbarService.ToggleSidebar();
        if (OnSidebarToggle.HasDelegate)
        {
            await OnSidebarToggle.InvokeAsync();
        }
    }

    private void HandleSearch(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        // Implement debounce search logic if needed
    }

    private void ToggleNotifications()
    {
        _isNotificationOpen = !_isNotificationOpen;
        _isProfileOpen = false;
    }

    private void ToggleProfile()
    {
        _isProfileOpen = !_isProfileOpen;
        _isNotificationOpen = false;
    }


    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<Topbar>? _dotNetHelper;
    private bool _isDisposed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            try
            {
                _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/LHA.BlazorWasm.Components/js/topbar.js");
                if (!_isDisposed)
                {
                    await _jsModule.InvokeVoidAsync("initTopbar", _dotNetHelper);
                }
            }
            catch (ObjectDisposedException)
            {
                // Component disposed before JS interop could finish.
            }
            catch (JSDisconnectedException)
            {
                // Browser connection lost.
            }
            catch (TaskCanceledException)
            {
                // Interop cancelled.
            }
        }
    }

    [JSInvokable]
    public void OpenCommandPalette()
    {
        // Logic to open command palette
        ToastNotification.Info("Command Palette (Ctrl + K) triggered!");
    }

    [JSInvokable]
    public void CloseDropdowns()
    {
        _isNotificationOpen = false;
        _isProfileOpen = false;
        StateHasChanged();
    }

    public override void Dispose()
    {
        base.Dispose();
        _isDisposed = true;
        TopbarService.State.OnStateChanged -= StateHasChanged;

        _dotNetHelper?.Dispose();

        if (_jsModule != null)
        {
            _ = _jsModule.DisposeAsync().AsTask();
        }

        GC.SuppressFinalize(this);
    }
}
