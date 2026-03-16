namespace LHA.Ddd.Domain;

/// <summary>
/// Base exception for domain / business-rule violations that should be
/// communicated to the client with a machine-readable <see cref="Code"/>
/// and an optional HTTP <see cref="StatusCode"/> hint.
/// <para>
/// <b>Localization pattern:</b> omit <paramref name="message"/> and call
/// <see cref="WithData"/> to supply format arguments.  The presentation layer
/// (<c>BusinessExceptionLocalizer</c>) looks up <see cref="Code"/> as a key
/// across all registered localization resources and formats the template with
/// <see cref="Args"/>.  When no translation is found it falls back to
/// <see cref="Exception.Message"/> (which equals <see cref="Code"/> when no
/// explicit message was provided).
/// </para>
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// Machine-readable error code (e.g. <c>"IDENTITY:LOGIN_FAILED"</c>,
    /// <c>"Identity_User_UserName_Duplicate_Error_Message_Entry"</c>).
    /// Also used as the localization resource lookup key.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Suggested HTTP status code. Default is <c>400</c> (Bad Request).
    /// Override in derived types for 409 (Conflict), 422 (Unprocessable), etc.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Positional format arguments for the localized message template.
    /// Maps to <c>{0}</c>, <c>{1}</c>, … placeholders in the JSON resource file.
    /// Set via <see cref="WithData"/>.
    /// </summary>
    public object[]? Args { get; private set; }

    /// <param name="code">
    /// Machine-readable code that doubles as the localization resource key.
    /// </param>
    /// <param name="message">
    /// Optional hard-coded fallback message. When omitted, <paramref name="code"/>
    /// is used as the message, keeping the fallback human-readable.
    /// </param>
    /// <param name="statusCode">HTTP status hint. Defaults to 400.</param>
    /// <param name="innerException">Optional inner exception.</param>
    public BusinessException(
        string code,
        string? message = null,
        int statusCode = 400,
        Exception? innerException = null)
        : base(message ?? code, innerException)
    {
        Code = code;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Attaches positional format arguments used by the localization layer to
    /// render the message template (<c>{0}</c>, <c>{1}</c>, …).
    /// </summary>
    /// <returns>This instance for fluent chaining.</returns>
    public BusinessException WithData(params object[] args)
    {
        Args = args;
        return this;
    }
}

/// <summary>
/// Thrown when a concurrency conflict is detected (optimistic concurrency / ETag mismatch).
/// Maps to HTTP <c>409 Conflict</c>.
/// </summary>
public sealed class ConcurrencyException : BusinessException
{
    public ConcurrencyException(string? message = null, Exception? innerException = null)
        : base("CONCURRENCY_CONFLICT", message, 409, innerException) { }
}

/// <summary>
/// Thrown when a uniqueness constraint is violated at the domain level.
/// Maps to HTTP <c>409 Conflict</c>.
/// </summary>
public sealed class DuplicateException : BusinessException
{
    public DuplicateException(string code, string? message = null, Exception? innerException = null)
        : base(code, message, 409, innerException) { }
}

/// <summary>
/// Thrown when an input validation rule fails in the domain or application layer.
/// Maps to HTTP <c>422 Unprocessable Entity</c>.
/// </summary>
public sealed class ValidationException : BusinessException
{
    /// <summary>Per-field validation details.</summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors)
        : base("VALIDATION_FAILED", "One or more validation errors occurred.", 422)
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : this([new ValidationError(field, message)]) { }
}

/// <summary>
/// Represents a single field-level validation error.
/// </summary>
/// <param name="Field">The field or property that failed validation.</param>
/// <param name="Message">Human-readable description of the validation failure.</param>
public sealed record ValidationError(string Field, string Message);
