using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.DistributedLocking;
using LHA.EntityFrameworkCore;
using LHA.EventBus;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAInMemoryEventBus();

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

// ── Event handlers ───────────────────────────────────────────────
// Register integration event handlers here as they are implemented.
// Example: builder.Services.AddTransient<IEventHandler<SomeEvent>, SomeEventHandler>();

using var host = builder.Build();
await host.RunAsync();
