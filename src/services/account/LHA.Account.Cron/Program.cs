using LHA.Account.Cron;
using LHA.Account.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── EF Core ─────────────────────────────────────────────────────
builder.Services.AddAccountEntityFrameworkCore(connectionString);

// ── Scheduled jobs ──────────────────────────────────────────────
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
