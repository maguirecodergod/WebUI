using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus.Kafka;
using LHA.Mega.EntityFrameworkCore;
using LHA.MessageBroker.Kafka;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ──────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();

// ── EF Core (for outbox/inbox tables) ───────────────────────────
builder.Services.AddMegaEntityFrameworkCore(options =>
{
    options.Configure<MegaDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Kafka infrastructure ────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
});

// ── Kafka event bus (consumer mode) ─────────────────────────────
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "mega-consumer";
        eventBus.ApplicationName = "MegaConsumer";
    },
    kafka =>
    {
        kafka.DefaultTopic = "mega-events";
    });

// ── Subscribe to topics (add as needed) ─────────────────────────
// builder.Services.AddKafkaEventConsumer("some-topic");

var host = builder.Build();
host.Run();
