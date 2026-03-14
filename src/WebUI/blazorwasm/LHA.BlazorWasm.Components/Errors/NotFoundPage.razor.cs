using LHA.BlazorWasm.Services.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Errors;

/// <summary>
/// A reusable enterprise-grade NotFound (404) Page component.
/// Designed for SaaS applications with premium aesthetics and localization support.
/// </summary>
public partial class NotFoundPage : ComponentBase, IDisposable
{
    [Inject] private ILocalizationService L { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// Gets or sets the title of the error page.
    /// Default: Localized "Errors.NotFound.Title"
    /// </summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the message description below the title.
    /// Default: Localized "Errors.NotFound.Description"
    /// </summary>
    [Parameter] public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the illustration or icon.
    /// Default: "fas fa-search"
    /// </summary>
    [Parameter] public string IllustrationIcon { get; set; } = "fas fa-search";

    /// <summary>
    /// Gets or sets a value indicating whether to show the "Go Home" button.
    /// </summary>
    [Parameter] public bool ShowHomeButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the "Go Back" button.
    /// </summary>
    [Parameter] public bool ShowBackButton { get; set; } = true;

    /// <summary>
    /// Gets or sets the URL for the home button.
    /// Default: "/"
    /// </summary>
    [Parameter] public string HomeUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets a custom action for the back button.
    /// If null, it defaults to browser history back.
    /// </summary>
    [Parameter] public EventCallback? BackAction { get; set; }

    /// <summary>
    /// Gets or sets additional actions or content to be displayed below the default buttons.
    /// </summary>
    [Parameter] public RenderFragment? ExtraActions { get; set; }

    /// <summary>
    /// Gets or sets the search slot content.
    /// </summary>
    [Parameter] public RenderFragment? SearchSlot { get; set; }

    private string DisplayTitle => Title ?? L.L("Errors.NotFound.Title");
    private string DisplayMessage => Message ?? L.L("Errors.NotFound.Description");
    private string GoHomeText => L.L("Errors.NotFound.GoHome");
    private string GoBackText => L.L("Errors.NotFound.GoBack");

    protected override void OnInitialized()
    {
        L.OnLanguageChanged += HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        L.OnLanguageChanged -= HandleLanguageChanged;
    }

    private async Task HandleBackAsync()
    {
        if (BackAction.HasValue && BackAction.Value.HasDelegate)
        {
            await BackAction.Value.InvokeAsync();
        }
        else
        {
            await JS.InvokeVoidAsync("history.back");
        }
    }

    private void HandleHome()
    {
        NavigationManager.NavigateTo(HomeUrl);
    }
}
