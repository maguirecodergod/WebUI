using Microsoft.AspNetCore.Routing;

namespace LHA.Notification.HttpApi;

/// <summary>
/// Notification endpoints.
/// </summary>
public static class NotificationEndpoints
{
    /// <summary>
    /// Maps the notification endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapNotificationMessageEndpoints();
        endpoints.MapDeviceEndpoints();
        endpoints.MapTemplateEndpoints();
        endpoints.MapBatchEndpoints();
        endpoints.MapUserPreferenceEndpoints();
        endpoints.MapChannelConfigurationEndpoints();
        endpoints.MapNotificationStatsEndpoints();
        endpoints.MapNotificationAuditLogEndpoints();
        endpoints.MapNotificationBackgroundJobEndpoints();

        return endpoints;
    }
}
