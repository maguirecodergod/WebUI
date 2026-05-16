using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using LHA.Auditing;
using LHA.BackgroundWorker.Hangfire;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.EventBus.Kafka;
using LHA.MessageBroker.Kafka;
using LHA.MultiTenancy;
using LHA.Notification.Application.DependencyInjection;
using LHA.Notification.Cron.Workers;
using LHA.Notification.Infrastructure;
using LHA.Scheduling.Hangfire;
using LHA.UnitOfWork;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ──────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddHttpClient();

// ── Kafka infrastructure ────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
});

// ── Kafka event bus (consumer/worker mode with inbox) ────────────
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "notification-cron";
        eventBus.ApplicationName = "NotificationCron";
        eventBus.EnableInbox = true;
    },
    kafka =>
    {
        kafka.DefaultTopic = LHA.Shared.Contracts.EventTopics.NotificationEvents;
    });

// ── Module services (Application + Infrastructure) ──────────────
builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

// ── Scheduled jobs (Hangfire + MongoDB storage) ─────────────────
builder.Services.AddLHAHangfireScheduling(options =>
{
    options.ConfigureHangfire = config =>
        config.UseMongoStorage(
            connectionString,
            "LHA_Notification_Hangfire",
            new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                },
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            });
});

// ── Register workers ──
builder.Services.AddLHAHangfireBackgroundWorker<BatchProgressSyncWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<CleanupDeliveredNotificationWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<DeactivateInactiveDevicesWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<ExpireOldNotificationsWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<GenerateDailyStatsWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<ProcessScheduledNotificationsWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<RetryFailedNotificationsWorker>();
builder.Services.AddLHAHangfireBackgroundWorker<UnreadCountReconciliationWorker>();

var host = builder.Build();
host.Run();
