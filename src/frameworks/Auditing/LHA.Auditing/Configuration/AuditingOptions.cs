using System;
using System.Collections.Generic;
using System.Threading;

namespace LHA.Auditing;

[Flags]
public enum AuditMode
{
    None = 0,
    HttpPipeline = 1, // Only HTTP API audit
    DataChange = 2,   // Only EF Core Entity Change audit
    All = HttpPipeline | DataChange
}

/// <summary>
/// Configuration options for the LHA auditing infrastructure.
/// </summary>
public sealed class AuditingOptions
{
    /// <summary>
    /// Master switch for auditing. Default: <c>true</c>.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Enabled audit modes. Default: <see cref="AuditMode.None"/>.
    /// </summary>
    public AuditMode Mode { get; set; } = AuditMode.None;

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
    /// Always persist an audit log when an exception occurs, regardless of other filters.
    /// Default: <c>true</c>.
    /// </summary>
    public bool AlwaysLogOnException { get; set; } = true;

    /// <summary>
    /// Whether to capture the request body in audit logs.
    /// Default: <c>false</c>.
    /// </summary>
    public bool CaptureRequestBody { get; set; }


    /// <summary>
    /// Whether to create audit logs for anonymous (unauthenticated) users.
    /// Default: <c>true</c>.
    /// </summary>
    public bool IsEnabledForAnonymousUsers { get; set; } = true;

    /// <summary>
    /// Whether to create audit logs for HTTP GET/HEAD requests.
    /// Default: <c>false</c>.
    /// </summary>
    public bool IsEnabledForGetRequests { get; set; }

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
