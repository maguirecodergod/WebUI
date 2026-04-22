using LHA.Account.Consumer.EventHandlers;
using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.EventBus.Kafka;
using LHA.Mega.Domain.Shared.Events;
using LHA.MessageBroker.Kafka;
using LHA.MultiTenancy;
using LHA.UnitOfWork;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ──────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();

// ── EF Core ─────────────────────────────────────────────────────
builder.Services.AddAccountEntityFrameworkCore(connectionString);

// ── Kafka infrastructure ────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
    kafka.Consumer.AllowAutoCreateTopics = true;
});

// ── Kafka event bus (consumer mode) ─────────────────────────────
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "account-consumer";
        eventBus.ApplicationName = "AccountConsumer";
        eventBus.EnableInbox = true;
    },
    kafka =>
    {
        kafka.DefaultTopic = "mega-events";
    });

// ── Event handlers ──────────────────────────────────────────────
builder.Services.AddScoped<IEventHandler<MegaAccountCreatedEvent>, MegaAccountCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<MegaAccountUpdatedEvent>, MegaAccountUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<MegaAccountDeletedEvent>, MegaAccountDeletedEventHandler>();

// ── Consume from Mega events topic ──────────────────────────────
builder.Services.AddKafkaEventConsumer("mega-events");

var host = builder.Build();
host.Run();
