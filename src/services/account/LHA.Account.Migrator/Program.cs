using LHA.Account.Migrator.Database;
using LHA.Account.Migrator.Infrastructure;
using LHA.Account.Migrator.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json");

builder.AddAccountMigratorServices();

using var host = builder.Build();

var logger = host.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Account.Migrator");

await DatabaseMigrationRunner.RunAsync(host.Services, logger);
await DataSeederOrchestrator.RunAsync(host.Services, logger);
