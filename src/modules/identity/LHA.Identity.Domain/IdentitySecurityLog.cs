using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;

namespace LHA.Identity.Domain;

/// <summary>
/// Records a security-relevant action (login, logout, password change, etc.).
/// Immutable once created.
/// </summary>
public sealed class IdentitySecurityLog : CreationAuditedEntity<Guid>, IMultiTenant
{
    /// <inheritdoc />
    public Guid? TenantId { get; private init; }

    /// <summary>Application that recorded the log.</summary>
    public string? ApplicationName { get; private init; }

    /// <summary>Identity subsystem (e.g., "Identity", "IdentityExternal").</summary>
    public string? Identity { get; private init; }

    /// <summary>Action name (e.g., "LoginSucceeded").</summary>
    public string Action { get; private init; } = null!;

    /// <summary>User ID (nullable for anonymous actions).</summary>
    public Guid? UserId { get; private init; }

    /// <summary>User name at the time of the action.</summary>
    public string? UserName { get; private init; }

    /// <summary>Tenant name at the time of the action.</summary>
    public string? TenantName { get; private init; }

    /// <summary>OAuth client ID (if applicable).</summary>
    public string? ClientId { get; private init; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    public string? CorrelationId { get; private init; }

    /// <summary>Client IP address.</summary>
    public string? ClientIpAddress { get; private init; }

    /// <summary>Browser / user-agent information.</summary>
    public string? BrowserInfo { get; private init; }

    /// <summary>Serialized extra properties (JSON).</summary>
    public string? ExtraProperties { get; private init; }

    /// <summary>EF Core constructor.</summary>
    private IdentitySecurityLog() { }

    /// <summary>Creates a new security log entry.</summary>
    public IdentitySecurityLog(
        Guid id,
        string action,
        Guid? tenantId = null,
        string? applicationName = null,
        string? identity = null,
        Guid? userId = null,
        string? userName = null,
        string? tenantName = null,
        string? clientId = null,
        string? correlationId = null,
        string? clientIpAddress = null,
        string? browserInfo = null,
        string? extraProperties = null)
    {
        Id = id;
        TenantId = tenantId;
        ApplicationName = Truncate(applicationName, IdentitySecurityLogConsts.MaxApplicationNameLength);
        Identity = Truncate(identity, IdentitySecurityLogConsts.MaxIdentityLength);
        Action = Truncate(action, IdentitySecurityLogConsts.MaxActionLength)!;
        UserId = userId;
        UserName = Truncate(userName, IdentitySecurityLogConsts.MaxUserNameLength);
        TenantName = Truncate(tenantName, IdentitySecurityLogConsts.MaxTenantNameLength);
        ClientId = Truncate(clientId, IdentitySecurityLogConsts.MaxClientIdLength);
        CorrelationId = Truncate(correlationId, IdentitySecurityLogConsts.MaxCorrelationIdLength);
        ClientIpAddress = Truncate(clientIpAddress, IdentitySecurityLogConsts.MaxClientIpAddressLength);
        BrowserInfo = Truncate(browserInfo, IdentitySecurityLogConsts.MaxBrowserInfoLength);
        ExtraProperties = Truncate(extraProperties, IdentitySecurityLogConsts.MaxExtraPropertiesLength);
    }

    private static string? Truncate(string? value, int maxLength)
        => value is not null && value.Length > maxLength ? value[..maxLength] : value;
}
