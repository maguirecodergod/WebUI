using LHA.AuditLog.Domain;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;
using LHA.Caching.StackExchangeRedis;
using LHA.EntityFrameworkCore;
using LHA.EntityFrameworkCore.Auditing;
using LHA.EntityFrameworkCore.MongoDB.Provisioning;
using LHA.EventBus;
using LHA.MultiTenancy.Provisioning;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;

using LHA.Notification.Infrastructure.Channels.Email.AwsSes;
using LHA.Notification.Infrastructure.Channels.Email.SendGrid;
using LHA.Notification.Infrastructure.Channels.Email.Smtp;
using LHA.Notification.Infrastructure.Channels.InApp;
using LHA.Notification.Infrastructure.Channels.Push.Apns;
using LHA.Notification.Infrastructure.Channels.Push.Fcm;
using LHA.Notification.Infrastructure.Channels.Sms.AwsSns;
using LHA.Notification.Infrastructure.Channels.Sms.Twilio;
using LHA.Notification.Infrastructure.Channels.WebPush;
using LHA.Notification.Infrastructure.Persistences;
using LHA.Notification.Infrastructure.Persistences.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;

namespace LHA.Notification.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
            services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
            services.Configure<SendGridProviderSettings>(configuration.GetSection(SendGridProviderSettings.SectionName));
            services.Configure<TwilioProviderSettings>(configuration.GetSection(TwilioProviderSettings.SectionName));
            services.Configure<FcmProviderSettings>(configuration.GetSection(FcmProviderSettings.SectionName));
            services.Configure<ApnsProviderSettings>(configuration.GetSection(ApnsProviderSettings.SectionName));
            services.Configure<AwsSesProviderSettings>(configuration.GetSection(AwsSesProviderSettings.SectionName));
            services.Configure<SmtpProviderSettings>(configuration.GetSection(SmtpProviderSettings.SectionName));
            services.Configure<AwsSnsProviderSettings>(configuration.GetSection(AwsSnsProviderSettings.SectionName));
            services.Configure<WebPushProviderSettings>(configuration.GetSection(WebPushProviderSettings.SectionName));

            // Unify connection string resolution: prefer ConnectionStrings:Default, then MongoDB:ConnectionString
            string connectionString = configuration.GetConnectionString("Default") 
                ?? configuration["MongoDB:ConnectionString"] 
                ?? "mongodb://admin:admin@localhost:27017/LHA_Notification?authSource=admin";
            
            services.AddNotificationEntityFrameworkCore(connectionString);

            var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
            string redisConnection = configuration.GetConnectionString("Redis") 
                ?? redisSettings?.ConnectionString 
                ?? "localhost:6379";
            
            string localDefaultChannelPrefix = "notification";
            string channelPrefix = redisSettings?.InstanceName?.TrimEnd(':') ?? localDefaultChannelPrefix;

            services.AddLHAStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = channelPrefix;
            });

            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    // Removed JsonStringEnumConverter for performance testing with integer enums
                })
                .AddStackExchangeRedis(redisConnection, options =>
                {
                    options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal(channelPrefix);
                });

            services.AddScoped<INotificationRepository, EfCoreNotificationRepository>();
            services.AddScoped<IDeviceRepository, EfCoreDeviceRepository>();
            services.AddScoped<ITemplateRepository, EfCoreTemplateRepository>();
            services.AddScoped<INotificationBatchRepository, EfCoreNotificationBatchRepository>();
            services.AddScoped<IUserPreferenceRepository, EfCoreUserPreferenceRepository>();
            services.AddScoped<IChannelConfigurationRepository, EfCoreChannelConfigurationRepository>();

            services.AddScoped<IUnreadCountCache, UnreadCountCache>();

            services.AddScoped<IChannelProviderFactory, ChannelProviderFactory>();

            services.AddScoped<INotificationHubContext, NotificationHubContext>();
            services.AddScoped<IRealTimeNotifier, RealTimeNotifier>();

            services.AddScoped<INotificationEventPublisher, NotificationEventPublisher>();

            services.AddScoped<IFcmPushProvider, FcmPushProvider>();
            services.AddScoped<IApnsPushProvider, ApnsPushProvider>();
            services.AddScoped<ISendGridEmailProvider, SendGridEmailProvider>();
            services.AddScoped<IAwsSesEmailProvider, AwsSesEmailProvider>();
            services.AddScoped<ISmtpEmailProvider, SmtpEmailProvider>();
            services.AddScoped<ITwilioSmsProvider, TwilioSmsProvider>();
            services.AddScoped<IAwsSnsSmsProvider, AwsSnsSmsProvider>();
            services.AddScoped<IWebPushProvider, WebPushChannelProvider>();
            services.AddScoped<IInAppProvider, InAppChannelProvider>();

            services.AddSingleton<SmtpConnectionPool>();
            services.AddSingleton<VapidKeyProvider>();
            services.AddScoped<FcmResponseMapper>();
            services.AddScoped<FcmMessageBuilder>();
            services.AddScoped<ApnsMessageBuilder>();
            services.AddScoped<ApnsJwtTokenProvider>();
            services.AddScoped<SendGridRequestBuilder>();

            services.AddScoped<ITemplateEngine, HandlebarsTemplateEngine>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();

            services.AddResiliencePipeline("ChannelProviderPipeline", builder =>
            {
                builder.AddConcurrencyLimiter(50);

                builder.AddTimeout(TimeSpan.FromSeconds(10));

                builder.AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    MinimumThroughput = 5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(60),
                    ShouldHandle = new Polly.PredicateBuilder().Handle<Exception>()
                });

                builder.AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    ShouldHandle = new Polly.PredicateBuilder().Handle<Exception>(),
                    MaxRetryAttempts = 3,
                    BackoffType = Polly.DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(1),
                    UseJitter = true
                });
            });
            return services;
        }


        private static IServiceCollection AddNotificationEntityFrameworkCore(
            this IServiceCollection services,
            string connectionString)
        {
            // 0) Register Data Auditing Interceptor
            services.AddScoped<DataAuditingSaveChangesInterceptor>();

            // 1) Register the unified NotificationDbContext.
            services.AddLhaDbContext<NotificationDbContext>(options =>
            {
                options.Configure<NotificationDbContext>(ctx =>
                    ctx.DbContextOptions.UseMongoDB(connectionString));

                // Mapping AuditLogDbContext to NotificationDbContext for UoW consistency
                options.ReplaceDbContext<AuditLogDbContext, NotificationDbContext>();
            });

            // 2) Register Audit Log EF Core storage with MongoDB provider.
            services.AddAuditLogEntityFrameworkCore(builder =>
            {
                builder.UseMongoDb(); // REQUIRED: Configure model mappings for MongoDB
                builder.UseAll();

                builder.ConfigureDbContext(options =>
                {
                    // Explicitly configure AuditLogDbContext for MongoDB resolution
                    options.Configure<AuditLogDbContext>(ctx =>
                        ctx.DbContextOptions.UseMongoDB(connectionString));
                });
            });


            // Re-register outbox/inbox stores for the unified NotificationDbContext.
            services.Replace(ServiceDescriptor.Scoped(typeof(IOutboxStore), typeof(EfCoreOutboxStore<NotificationDbContext>)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IInboxStore), typeof(EfCoreInboxStore<NotificationDbContext>)));

            // 3) Register Account specialized Audit Log Repositories
            services.AddScoped<IAuditLogRepository, EfCoreAuditLogRepository>();
            // services.AddScoped<IAuditLogActionRepository, EfCoreAuditLogActionRepository>();
            // services.AddScoped<IEntityChangeRepository, EfCoreEntityChangeRepository>();
            // services.AddScoped<IEntityPropertyChangeRepository, EfCoreEntityPropertyChangeRepository>();

            // 4) Register the Tenant Provisioning Migrator for NotificationDbContext
            services.AddTransient<ITenantDatabaseMigrator, MongoDbTenantDatabaseMigrator<NotificationDbContext>>();

            return services;
        }
    }
}
