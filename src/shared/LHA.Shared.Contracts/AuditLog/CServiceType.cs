using LHA.Shared.Domain.Attributes;

namespace LHA.Shared.Contracts.AuditLog;

/// <summary>
/// Identifies the microservice that originated an audit log entry.
/// </summary>
public enum CServiceType
{
    /// <summary>
    /// Account service — handles identity, authentication, and tenant management.
    /// </summary>
    [EnumMetadata(DisplayName = "ServiceType.Account", Icon = "bi bi-person-badge")]
    Account = 1,

    /// <summary>
    /// Notification service — manages push, email, SMS, and in-app notifications.
    /// </summary>
    [EnumMetadata(DisplayName = "ServiceType.Notification", Icon = "bi bi-bell")]
    Notification = 2,

    /// <summary>
    /// Mega service — core business platform integrating multiple modules.
    /// </summary>
    [EnumMetadata(DisplayName = "ServiceType.Mega", Icon = "bi bi-grid-3x3-gap")]
    Mega = 3,

    /// <summary>
    /// Movie service — manages movie content, catalog, and streaming metadata.
    /// </summary>
    [EnumMetadata(DisplayName = "ServiceType.Movie", Icon = "bi bi-film")]
    Movie = 4
}
