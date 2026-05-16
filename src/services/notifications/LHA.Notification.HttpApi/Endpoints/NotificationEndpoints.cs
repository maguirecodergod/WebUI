using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LHA.Notification.HttpApi;

public static class NotificationEndpoints
{
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
