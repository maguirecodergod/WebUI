using LHA.ApiGateway.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog Logger ──────────────────────────────────────────────────
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] [{TenantId}] {Message:lj}{NewLine}{Exception}");
});

// ── Gateway Services ───────────────────────────────────────────────
// Injects Yarp, JWT Bearer Auth, Rate Limiting, and CORS
builder.Services.AddGatewayInfrastructure(builder.Configuration);

// ── Build Application ─────────────────────────────────────────────
var app = builder.Build();

// ── Setup Gateway Pipeline ────────────────────────────────────────
// Maps Yarp endpoints, Auth middlewares, Rate limiters, and Custom Middlewares
app.UseGatewayPipeline();

app.Run();
