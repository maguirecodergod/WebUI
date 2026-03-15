using Microsoft.Extensions.Logging;

namespace LHA.BlazorWasm.Services.ErrorHandling;

/// <summary>
/// Default implementation of <see cref="IErrorReporter"/>.
/// Generates a short error ID, logs the error, and returns a structured report.
/// </summary>
public sealed class ErrorReporter : IErrorReporter
{
    private readonly ILogger<ErrorReporter> _logger;

    public ErrorReporter(ILogger<ErrorReporter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public ErrorReport ReportError(Exception exception, string? url = null)
    {
        var errorId = Guid.NewGuid().ToString("N")[..8];
        var timestamp = DateTime.UtcNow;

        var report = new ErrorReport
        {
            ErrorId = errorId,
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            Timestamp = timestamp,
            Url = url
        };

        // Log using standard ILogger instead of hardcoded Console
        // This allows logging to Application Insights, Seq, Serilog, etc.
        _logger.LogError(exception, "[GlobalErrorBoundary] Error {ErrorId} at {Url}", errorId, url ?? "N/A");

        return report;
    }
}
