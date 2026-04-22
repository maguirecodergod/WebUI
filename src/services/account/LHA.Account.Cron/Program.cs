using LHA.Account.Cron;
using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.MultiTenancy;
using LHA.UnitOfWork;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ──────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
// ── EF Core ─────────────────────────────────────────────────────
builder.Services.AddAccountEntityFrameworkCore(connectionString);

// ── Scheduled jobs ──────────────────────────────────────────────
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
