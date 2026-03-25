using LHA.AspNetCore;
using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.HttpApi;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAExceptionHandler();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddAuditLogApplication();
builder.Services.AddAuditLogEntityFrameworkCore(auditBuilder =>
{
    // ─── AUDIT LOG STORE MODE EXAMPLES ───
    // Uncomment ONE of the following modes to test different audit setups
    // when creating your schema and writing logs:

    // 1. All Mode (Default): Uses both Relational Structured Logs & Pipeline Logs
    auditBuilder.UseAll(); 

    // 2. Data Audit Only: Relational Data Action and Entity Logs (Pipeline ignored)
    //auditBuilder.UseDataAuditOnly();

    // 3. Pipeline Only: High-throughput API logs (Data Audit tables ignored)
    //auditBuilder.UsePipelineOnly();
    auditBuilder.ConfigureDbContext(options =>
                {
                    options.Configure<AuditLogDbContext>(ctx =>
                        ctx.DbContextOptions.UseNpgsql(connectionString));
                });
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHAUnitOfWork();
app.UseLHASwagger();

// ── Endpoints ────────────────────────────────────────────────────
app.MapAuditLogEndpoints();

app.Run();
