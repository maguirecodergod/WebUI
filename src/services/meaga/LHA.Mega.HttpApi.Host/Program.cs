using System.Text;
using LHA.AspNetCore;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus.Kafka;
using LHA.Mega.Application;
using LHA.Mega.EntityFrameworkCore;
using LHA.Mega.HttpApi;
using LHA.MessageBroker.Kafka;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();

// ── Kafka event bus ──────────────────────────────────────────────
builder.Services.AddLHAKafka(kafka =>
{
    kafka.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
});
builder.Services.AddLHAKafkaEventBus(
    eventBus =>
    {
        eventBus.ConsumerGroup = "mega-service";
        eventBus.ApplicationName = "MegaService";
        eventBus.EnableOutbox = true;
    },
    kafka =>
    {
        kafka.DefaultTopic = "mega-events";
    });
builder.Services.AddKafkaOutboxProcessor();

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAExceptionHandler();

// ── JWT configuration ────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
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
    });

builder.Services.AddLHAPermissionAuthorization();

// ── Module services (Application + EF Core) ──────────────────────
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

builder.Services.AddMegaApplication();
builder.Services.AddMegaEntityFrameworkCore(options =>
{
    options.Configure<MegaDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHAUnitOfWork();
app.UseLHASwagger();
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ────────────────────────────────────────────────────
app.MapMegaEndpoints();

app.Run();
