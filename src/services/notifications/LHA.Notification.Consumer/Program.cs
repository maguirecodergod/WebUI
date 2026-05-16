using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus.Kafka;
using LHA.MessageBroker.Kafka;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using LHA.EventBus;
using LHA.Notification.Application.DependencyInjection;
using LHA.Notification.Infrastructure;
using LHA.Shared.Contracts;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ──────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddHttpClient();

// ── Module services (Application + Infrastructure) ──────────────
builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

// ── Kafka infrastructure ────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
    kafka.Consumer.AllowAutoCreateTopics = true;
});

// ── Kafka event bus (consumer mode with inbox) ──────────────────
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "notification-consumer";
        eventBus.ApplicationName = "NotificationConsumer";
        eventBus.EnableInbox = true;
    },
    kafka =>
    {
        kafka.DefaultTopic = EventTopics.NotificationEvents;
    });

// ── Consume from notification events topic ─────────────────────
builder.Services.AddKafkaEventConsumer(EventTopics.NotificationEvents);
builder.Services.AddKafkaEventConsumer(EventTopics.AccountEvents);

// ── Register Event Handlers ─────────────────────────────────────
builder.Services.AddTransient<IEventHandler<LHA.Identity.Application.Contracts.UserCreatedEto>, LHA.Notification.Consumer.Handlers.UserCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<LHA.Identity.Application.Contracts.LoginSucceededEto>, LHA.Notification.Consumer.Handlers.LoginSucceededEventHandler>();

var host = builder.Build();
host.Run();
