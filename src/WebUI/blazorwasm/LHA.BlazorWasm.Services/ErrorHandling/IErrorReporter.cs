namespace LHA.BlazorWasm.Services.ErrorHandling;

/// <summary>
/// Structured error report generated when an unhandled exception is caught.
/// </summary>
public sealed record ErrorReport
{
    /// <summary>Short unique identifier for this error occurrence.</summary>
    public required string ErrorId { get; init; }

    /// <summary>The exception message.</summary>
    public required string Message { get; init; }

    /// <summary>Full stack trace, if available.</summary>
    public string? StackTrace { get; init; }

    /// <summary>UTC timestamp when the error was captured.</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>The URL the user was on when the error occurred.</summary>
    public string? Url { get; init; }
}

/// <summary>
/// Abstraction for reporting and structuring unhandled exceptions.
/// Designed for future extensibility (e.g., sending to a backend API).
/// </summary>
public interface IErrorReporter
{
    /// <summary>
    /// Processes an exception into a structured <see cref="ErrorReport"/>,
    /// logs it to the console, and returns the report for UI display.
    /// </summary>
    /// <param name="exception">The caught exception.</param>
    /// <param name="url">The current page URL.</param>
    /// <returns>A structured error report.</returns>
    ErrorReport ReportError(Exception exception, string? url = null);
}
