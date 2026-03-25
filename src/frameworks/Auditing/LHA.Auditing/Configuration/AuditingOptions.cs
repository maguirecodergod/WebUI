namespace LHA.Auditing;

/// <summary>
/// Configuration options for the core LHA auditing infrastructure.
/// </summary>
public sealed class AuditingOptions
{
    /// <summary>
    /// Master switch for auditing. Default: <c>true</c>.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Application/service name stamped on audit log entries.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// When <c>true</c>, exceptions during audit log persistence are logged as warnings
    /// rather than propagated. Default: <c>true</c>.
    /// </summary>
    public bool HideErrors { get; set; } = true;

    /// <summary>
    /// Whether to capture the request body in audit logs via the HTTP Middleware.
    /// Default: <c>false</c>.
    /// </summary>
    public bool CaptureRequestBody { get; set; }

    /// <summary>
    /// Types to ignore during parameter serialization and entity history tracking
    /// (e.g., <see cref="System.IO.Stream"/>, <see cref="System.Threading.CancellationToken"/>).
    /// </summary>
    public List<Type> IgnoredTypes { get; } =
    [
        typeof(System.IO.Stream),
        typeof(System.Linq.Expressions.Expression),
        typeof(CancellationToken)
    ];

    /// <summary>
    /// JSON property names (case-insensitive) whose values will be replaced with "***"
    /// in audit log parameters and request bodies. Default covers common credential fields.
    /// </summary>
    public HashSet<string> SensitivePropertyNames { get; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "password", "newPassword", "oldPassword", "confirmPassword",
            "secret", "clientSecret",
            "token", "accessToken", "refreshToken", "idToken",
            "apiKey", "apiSecret",
            "creditCard", "cardNumber", "cvv", "cvc",
            "ssn", "pin", "otp"
        };
}
