using LHA.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.TenantManagement.BackgroundWorker;

/// <summary>
/// Periodic worker that permanently purges soft-deleted tenants
/// whose deletion time exceeds the retention period.
/// </summary>
public sealed class TenantCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<TenantCleanupWorker> logger) : BackgroundService
{
    /// <summary>How often to run (default: every 6 hours).</summary>
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    /// <summary>How long to retain soft-deleted tenants before hard-delete (default: 30 days).</summary>
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TenantCleanupWorker started. Interval={Interval}, Retention={Retention}",
            Interval, RetentionPeriod);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error during tenant cleanup.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();

        var cutoff = DateTimeOffset.UtcNow - RetentionPeriod;

        // Bypass soft-delete filter
        dbContext.IsSoftDeleteFilterEnabled = false;

        var expiredTenants = await dbContext.Tenants
            .Where(t => t.IsDeleted && t.DeletionTime != null && t.DeletionTime < cutoff)
            .ToListAsync(ct);

        if (expiredTenants.Count == 0)
        {
            logger.LogDebug("No expired soft-deleted tenants to clean up.");
            return;
        }

        // Hard-delete: remove connection strings and tenant rows permanently
        foreach (var tenant in expiredTenants)
        {
            var connStrings = await dbContext.TenantConnectionStrings
                .Where(cs => cs.TenantId == tenant.Id)
                .ToListAsync(ct);

            dbContext.TenantConnectionStrings.RemoveRange(connStrings);
            dbContext.Tenants.Remove(tenant);
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Purged {Count} expired soft-deleted tenant(s).", expiredTenants.Count);
    }
}
