using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;


namespace LHA.BlazorWasm.Components.Topbar;

public partial class Topbar : LHAComponentBase, IDisposable
{
    [Inject] public ITopbarService TopbarService { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Parameter] public RenderFragment? LeftContent { get; set; }
    [Parameter] public RenderFragment? CenterContent { get; set; }
    [Parameter] public RenderFragment? RightContent { get; set; }
    [Parameter] public bool IsSticky { get; set; } = true;


    [Parameter] public string AppName { get; set; } = "LHA WebUI";
    [Parameter] public string? LogoSvg { get; set; }

    [Parameter] public EventCallback OnSidebarToggle { get; set; }

    private bool _isNotificationOpen;
    private bool _isProfileOpen;
    private bool _isSearchOpen;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        TopbarService.State.OnStateChanged += StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            await TopbarService.LoadUserProfileAsync();
        }
    }

    // private async Task HandleSidebarToggle()
    // {
    //     TopbarService.ToggleSidebar();
    //     if (OnSidebarToggle.HasDelegate)
    //     {
    //         await OnSidebarToggle.InvokeAsync();
    //     }
    // }



    private void ToggleNotifications()
    {
        _isNotificationOpen = !_isNotificationOpen;
        _isProfileOpen = false;
    }

    private void ToggleProfile()
    {
        _isProfileOpen = !_isProfileOpen;
        _isNotificationOpen = false;
        _isSearchOpen = false;
    }

    private async Task HandleLogout()
    {
        await TopbarService.LogoutAsync();
        Navigation.NavigateTo("/login");
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
                _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", $"/_content/LHA.BlazorWasm.Components/js/topbar.js?t={DateTime.Now.Ticks}");
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
        _isSearchOpen = true;
        _isProfileOpen = false;
        _isNotificationOpen = false;
        StateHasChanged();
    }

    [JSInvokable]
    public void CloseDropdowns()
    {
        _isNotificationOpen = false;
        _isProfileOpen = false;
        _isSearchOpen = false;
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
