using Grpc.Net.Client;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Grpc.Contracts.Services.Account.V1;
using LHA.Shared.Contracts.Tax;
using LHA.Tax.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHAInMemoryEventBus();
builder.Services.AddLHADistributedLocking();

// ── Module services (Application + EF Core) ──────────────────────
// builder.Services.AddTaxApplication();
builder.Services.AddTaxEntityFrameworkCore(builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string."));

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Tax.Migrator");

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

try
{
    await ApplyMigrationsAsync(host.Services, logger, cancellation.Token);
    await RegisterPermissionsAsync(builder.Configuration, logger, cancellation.Token);
    logger.LogInformation("Tax migrator completed successfully.");
    return;
}
catch (OperationCanceledException)
{
    logger.LogWarning("Tax migrator canceled.");
    Environment.ExitCode = 2;
}
catch (Exception ex)
{
    logger.LogError(ex, "Tax migrator failed.");
    Environment.ExitCode = 1;
}

static async Task ApplyMigrationsAsync(
    IServiceProvider services,
    ILogger logger,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Applying Tax Service migrations...");

    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TaxDbContext>();

    // Apply EF Core migrations
    await dbContext.Database.MigrateAsync(cancellationToken);
    logger.LogInformation("Tax Service database migrations applied successfully.");
}

static async Task RegisterPermissionsAsync(
    IConfiguration configuration,
    ILogger logger,
    CancellationToken cancellationToken)
{
    var accountGrpcUrl = configuration["AccountService:GrpcUrl"] ?? "https://localhost:8150";
    logger.LogInformation("Registering Tax permissions with Account Service at {Url} (gRPC)...", accountGrpcUrl);

    using var channel = GrpcChannel.ForAddress(accountGrpcUrl);
    var client = new PermissionRegistrationService.PermissionRegistrationServiceClient(channel);
    var request = BuildPermissionRegistrationRequest();

    const int maxAttempts = 3;
    Exception? lastException = null;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var response = await client.RegisterPermissionsAsync(
                request,
                cancellationToken: cancellationToken);

            if (!response.Success)
            {
                throw new InvalidOperationException(
                    $"Account service rejected Tax permission registration: {response.Message}");
            }

            logger.LogInformation("Tax permissions registered successfully: {Message}", response.Message);
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            lastException = ex;
            var delay = TimeSpan.FromSeconds(attempt * 2);
            logger.LogWarning(ex,
                "Permission registration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds}s...",
                attempt, maxAttempts, delay.TotalSeconds);
            await Task.Delay(delay, cancellationToken);
        }
        catch (Exception ex)
        {
            lastException = ex;
            break;
        }
    }

    throw new InvalidOperationException(
        $"Unable to register Tax permissions after {maxAttempts} attempts.",
        lastException);
}

static RegisterPermissionsRequest BuildPermissionRegistrationRequest()
{
    var request = new RegisterPermissionsRequest
    {
        ServiceName = "Tax",
        GrantAllToAdminRole = true,
    };

    request.Groups.Add(new PermissionGroupDto
    {
        Name = TaxPermissions.GroupName,
        DisplayName = TaxPermissions.L.Group,
    });

    request.Permissions.AddRange(
    [
        // Jurisdictions
        new PermissionDefinitionDto { Name = TaxPermissions.Jurisdictions.Default, DisplayName = TaxPermissions.Jurisdictions.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Jurisdictions.Read, DisplayName = TaxPermissions.Jurisdictions.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Jurisdictions.Create, DisplayName = TaxPermissions.Jurisdictions.L.Create, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Jurisdictions.Update, DisplayName = TaxPermissions.Jurisdictions.L.Update, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Jurisdictions.Delete, DisplayName = TaxPermissions.Jurisdictions.L.Delete, GroupName = TaxPermissions.GroupName },

        // Tax Rates
        new PermissionDefinitionDto { Name = TaxPermissions.TaxRates.Default, DisplayName = TaxPermissions.TaxRates.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxRates.Read, DisplayName = TaxPermissions.TaxRates.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxRates.Create, DisplayName = TaxPermissions.TaxRates.L.Create, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxRates.Update, DisplayName = TaxPermissions.TaxRates.L.Update, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxRates.Delete, DisplayName = TaxPermissions.TaxRates.L.Delete, GroupName = TaxPermissions.GroupName },

        // Product Categories
        new PermissionDefinitionDto { Name = TaxPermissions.ProductCategories.Default, DisplayName = TaxPermissions.ProductCategories.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.ProductCategories.Read, DisplayName = TaxPermissions.ProductCategories.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.ProductCategories.Create, DisplayName = TaxPermissions.ProductCategories.L.Create, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.ProductCategories.Update, DisplayName = TaxPermissions.ProductCategories.L.Update, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.ProductCategories.Delete, DisplayName = TaxPermissions.ProductCategories.L.Delete, GroupName = TaxPermissions.GroupName },

        // Customer Profiles
        new PermissionDefinitionDto { Name = TaxPermissions.CustomerProfiles.Default, DisplayName = TaxPermissions.CustomerProfiles.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.CustomerProfiles.Read, DisplayName = TaxPermissions.CustomerProfiles.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.CustomerProfiles.Create, DisplayName = TaxPermissions.CustomerProfiles.L.Create, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.CustomerProfiles.Update, DisplayName = TaxPermissions.CustomerProfiles.L.Update, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.CustomerProfiles.Delete, DisplayName = TaxPermissions.CustomerProfiles.L.Delete, GroupName = TaxPermissions.GroupName },

        // Business Registrations
        new PermissionDefinitionDto { Name = TaxPermissions.BusinessRegistrations.Default, DisplayName = TaxPermissions.BusinessRegistrations.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.BusinessRegistrations.Read, DisplayName = TaxPermissions.BusinessRegistrations.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.BusinessRegistrations.Create, DisplayName = TaxPermissions.BusinessRegistrations.L.Create, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.BusinessRegistrations.Update, DisplayName = TaxPermissions.BusinessRegistrations.L.Update, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.BusinessRegistrations.Delete, DisplayName = TaxPermissions.BusinessRegistrations.L.Delete, GroupName = TaxPermissions.GroupName },

        // Tax Determination
        new PermissionDefinitionDto { Name = TaxPermissions.TaxDetermination.Default, DisplayName = TaxPermissions.TaxDetermination.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxDetermination.Calculate, DisplayName = TaxPermissions.TaxDetermination.L.Calculate, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxDetermination.Read, DisplayName = TaxPermissions.TaxDetermination.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxDetermination.History, DisplayName = TaxPermissions.TaxDetermination.L.History, GroupName = TaxPermissions.GroupName },

        // Tax Returns
        new PermissionDefinitionDto { Name = TaxPermissions.TaxReturns.Default, DisplayName = TaxPermissions.TaxReturns.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxReturns.Read, DisplayName = TaxPermissions.TaxReturns.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxReturns.Generate, DisplayName = TaxPermissions.TaxReturns.L.Generate, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.TaxReturns.File, DisplayName = TaxPermissions.TaxReturns.L.File, GroupName = TaxPermissions.GroupName },

        // Configuration
        new PermissionDefinitionDto { Name = TaxPermissions.Configuration.Default, DisplayName = TaxPermissions.Configuration.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Configuration.Read, DisplayName = TaxPermissions.Configuration.L.Read, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.Configuration.Manage, DisplayName = TaxPermissions.Configuration.L.Manage, GroupName = TaxPermissions.GroupName },

        // AuditLogs
        new PermissionDefinitionDto { Name = TaxPermissions.AuditLogs.Default, DisplayName = TaxPermissions.AuditLogs.L.Default, GroupName = TaxPermissions.GroupName },
        new PermissionDefinitionDto { Name = TaxPermissions.AuditLogs.Read, DisplayName = TaxPermissions.AuditLogs.L.Read, GroupName = TaxPermissions.GroupName }
    ]);

    return request;
}
