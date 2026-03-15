using LHA.BlazorWasm.Services.ErrorHandling;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LHA.BlazorWasm.Components.Errors;

/// <summary>
/// A global error boundary that catches all unhandled UI exceptions,
/// logs the error quietly, and provides a gentle Toast Warning instead of crashing the UI.
/// Inherits from Blazor's built-in <see cref="ErrorBoundary"/>.
/// </summary>
public partial class GlobalErrorBoundary : LhaErrorBoundaryBase, IDisposable
{
    [Inject] private IErrorReporter ErrorReporter { get; set; } = default!;

    protected override void OnInitialized()
    {
        Localizer.OnLanguageChanged += HandleLanguageChanged;
    }

    /// <summary>
    /// Called by the Blazor framework when a child component throws an unhandled exception.
    /// Processes the exception through the error reporter and shows a gentle warning toast notification.
    /// </summary>
    protected override Task OnErrorAsync(Exception exception)
    {
        var report = ErrorReporter.ReportError(exception, Navigation.Uri);

        try
        {
            var messageKey = GetMessageKeyForException(exception);

            // Show a gentle, localized toast warning with HTML support
            var safeMessage = $"{Localizer.L(messageKey)} <br/><small class='text-muted'>({Localizer.L("Errors.Global.ErrorId")}: {report.ErrorId})</small>";
            ToastNotification.Warning(safeMessage);
        }
        catch
        {
            // Swallow any toast failures
        }

        // Always auto-recover without replacing the screen! 
        // This discards the corrupted component state and re-renders the app boundary cleanly.
        Recover();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Maps specific exception types to friendly localized message keys.
    /// </summary>
    private string GetMessageKeyForException(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => "Errors.Global.NetworkError",
            TimeoutException or TaskCanceledException => "Errors.Global.TimeoutError",
            UnauthorizedAccessException => "Errors.Global.UnauthorizedError",
            NullReferenceException or ArgumentNullException => "Errors.Global.NullError",
            _ => "Errors.Global.ToastMessage"
        };
    }

    private void HandleLanguageChanged()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        Localizer.OnLanguageChanged -= HandleLanguageChanged;
    }
}
