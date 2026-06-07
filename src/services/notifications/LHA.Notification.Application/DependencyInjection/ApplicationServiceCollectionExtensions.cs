using LHA.AuditLog.Application;
using LHA.Notification.Application.Batches;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Application.Devices;
using LHA.Notification.Application.Notifications;
using LHA.Notification.Application.Preferences;
using LHA.Notification.Application.Services;
using LHA.Notification.Application.Statistics;
using LHA.Notification.Application.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Notification.Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationApplication(this IServiceCollection services)
        {
            services.AddScoped<IChannelConfigurationStore, ChannelConfigurationStore>();
            services.AddScoped<INotificationService, NotificationAppService>();

            services.AddScoped<IBatchService, BatchAppService>();
            services.AddScoped<IDeviceService, DeviceAppService>();
            services.AddScoped<IUserPreferenceService, UserPreferenceAppService>();
            services.AddScoped<ITemplateService, TemplateAppService>();
            services.AddScoped<INotificationStatsService, NotificationStatsAppService>();
            services.AddScoped<IChannelConfigurationAppService, ChannelConfigurationAppService>();
        
            services.AddAuditLogApplication();
            return services;
        }
    }
}
