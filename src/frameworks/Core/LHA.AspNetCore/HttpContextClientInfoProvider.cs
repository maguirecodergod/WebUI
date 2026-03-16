using LHA.Auditing;
using Microsoft.AspNetCore.Http;

namespace LHA.AspNetCore;

/// <summary>
/// ASP.NET Core implementation of <see cref="IClientInfoProvider"/> that reads
/// client information from <see cref="HttpContext"/>.
/// </summary>
internal sealed class HttpContextClientInfoProvider : IClientInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string? ClientIpAddress
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            // Prefer X-Forwarded-For header (reverse proxy scenario)
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                // X-Forwarded-For may contain multiple IPs; take the first (client)
                var firstIp = forwarded.Split(',', StringSplitOptions.TrimEntries)[0];
                return firstIp;
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }

    /// <inheritdoc />
    public string? BrowserInfo
    {
        get
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault();
            return string.IsNullOrWhiteSpace(userAgent) ? null : Truncate(userAgent, 512);
        }
    }

    /// <inheritdoc />
    public string? CorrelationId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null) return null;

            // Try common correlation ID headers
            return context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? context.Request.Headers["X-Request-Id"].FirstOrDefault()
                ?? context.TraceIdentifier;
        }
    }

    private static string Truncate(string value, int maxLength)
        => value.Length > maxLength ? value[..maxLength] : value;
}
