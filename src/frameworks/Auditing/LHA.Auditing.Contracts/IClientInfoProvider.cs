namespace LHA.Auditing;

/// <summary>
/// Provides client/request context information for audit logging.
/// <para>
/// The default implementation for ASP.NET Core reads from <c>HttpContext</c>.
/// Non-web hosts can register a no-op or custom implementation.
/// </para>
/// </summary>
public interface IClientInfoProvider
{
    /// <summary>Client IP address (e.g. from <c>X-Forwarded-For</c> or <c>RemoteIpAddress</c>).</summary>
    string? ClientIpAddress { get; }

    /// <summary>Browser / user-agent string.</summary>
    string? BrowserInfo { get; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    string? CorrelationId { get; }
}
