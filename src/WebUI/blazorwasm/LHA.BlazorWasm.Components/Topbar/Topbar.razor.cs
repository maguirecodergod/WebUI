using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Services.Theme;
using LHA.BlazorWasm.Components.Breadcrumb;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/LHA.BlazorWasm.Components/js/topbar.js");
            await _jsModule.InvokeVoidAsync("initTopbar", _dotNetHelper);
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

    public async void Dispose()
    {
        TopbarService.State.OnStateChanged -= StateHasChanged;
        
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
        _dotNetHelper?.Dispose();
    }
}
