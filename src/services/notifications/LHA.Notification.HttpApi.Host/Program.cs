using System.Text;
using LHA.AspNetCore;
using LHA.Auditing;
using LHA.Auditing.Interceptors;
using LHA.DistributedLocking;
using LHA.EventBus.Kafka;
using LHA.MessageBroker.Kafka;
using LHA.Grpc.Server;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LHA.Notification.Infrastructure.Persistences;
using LHA.Notification.Domain.Shared.Localization;
using LHA.Shared.Contracts;
using LHA.Caching.StackExchangeRedis;
using LHA.Notification.Infrastructure;
using LHA.Scheduling.Hangfire;
using Hangfire.Mongo;
using LHA.Notification.Application.DependencyInjection;
using LHA.Notification.HttpApi;
using LHA.Notification.HttpApi.Host.BackgroundJobs;
using LHA.Grpc.Client;
using LHA.Grpc.Contracts.Services.Account.V1;
using LHA.AspNetCore.Security;

var builder = WebApplication.CreateBuilder(args);

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAMultiTenancyHosting(options =>
{
    options.TenantResolvers.Add(new LHA.AspNetCore.Security.DomainTenantResolveContributor());
    options.TenantResolvers.Add(new LHA.AspNetCore.Security.HttpHeaderTenantResolveContributor());
});
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "LHA.Notification:";
});

// ── Kafka infrastructure ────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
});

// ── Kafka event bus (producer mode with outbox) ─────────────────
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "notification-service";
        eventBus.ApplicationName = "NotificationService";
        eventBus.EnableOutbox = true;
    },
    kafka =>
    {
        kafka.DefaultTopic = LHA.Shared.Contracts.EventTopics.NotificationEvents;
    });
builder.Services.AddKafkaOutboxProcessor();

// ── Module services (Application + EF Core) ──────────────────────
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

builder.Services.AddLHAHangfireScheduling(options =>
{
    options.ConfigureHangfire = config =>
        config.UseMongoStorage(builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string."));
    options.EnableServer = false; // API node only enqueues and monitors, does not process jobs
});

// ─── AUDIT LOG PRODUCER ───
// Use AuditingMode to control which audit producers run in this App.
// Make sure it matches the storage setup (CAuditLogStoreMode) in Account.EntityFrameworkCore!
builder.Services.AddLHAAuditLogging(
    mode: CAuditingMode.All,
    configureDataAudit: options =>
    {
        options.ApplicationName = "Notification";
        options.CaptureRequestBody = true;
    },
    configurePipeline: options =>
    {
        options.ServiceName = "Notification";
    });

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAAspNetCore(typeof(NotificationResource));

// ── JWT configuration ────────────────────────────────────────────
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddLHASecurityVersioning(builder.Configuration);

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Missing JWT SecretKey.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
        options.EnableSecurityVersionValidation();
    });

builder.Services.AddLHAPermissionAuthorization();
builder.Services.AddHostedService<PermissionRegistrationHostedService>();

// ── gRPC client registration ─────────────────────────────────────
builder.Services.AddLHAGrpcClientDefaults();
builder.Services.AddLHAGrpcClient<PermissionRegistrationService.PermissionRegistrationServiceClient>(
    builder.Configuration["AccountService:GrpcUrl"] ?? "https://localhost:8150");

// ── gRPC server ───────────────────────────────────────────────────
builder.Services.AddLHAGrpcServer();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuditGrpcInterceptor>();
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAAspNetCore();

app.UseLHASwagger();
app.UseAuthentication();
app.UseJwtTenantResolve(); // Resolve tenant from JWT tenant_id claim
// Apply chosen Audit Middlewares via the Facade (must match earlier setup)
app.UseLHAAuditLogging(mode: CAuditingMode.DataAudit);
app.UseAuthorization();

// ── Endpoints ────────────────────────────────────────────────────
app.MapNotificationEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapLHAGrpcInfrastructure();

// ── Auto Migration ───────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    // For MongoDB, MigrateAsync() is not supported. Use EnsureCreatedAsync() instead.
    await context.Database.EnsureCreatedAsync();
}

app.Run();
