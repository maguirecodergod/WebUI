using LHA.Mega.Cron;
using LHA.Mega.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── EF Core ─────────────────────────────────────────────────────
builder.Services.AddMegaEntityFrameworkCore(options =>
{
    options.Configure<MegaDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Scheduled jobs ──────────────────────────────────────────────
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
