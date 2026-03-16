using LHA.PermissionManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.PermissionManagement.BackgroundWorker;

/// <summary>
/// Periodic worker that cleans up orphaned permission grants
/// (grants referencing permission names that no longer exist).
/// </summary>
public sealed class PermissionGrantCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<PermissionGrantCleanupWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(12);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PermissionGrantCleanupWorker started. Interval={Interval}", Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error during permission grant cleanup.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<PermissionManagementDbContext>();

        // Find grants whose permission name doesn't match any definition
        var orphanedGrants = await dbContext.PermissionGrants
            .Where(g => !dbContext.PermissionDefinitions.Any(d => d.Name == g.Name))
            .ToListAsync(ct);

        if (orphanedGrants.Count == 0)
        {
            logger.LogDebug("No orphaned permission grants found.");
            return;
        }

        dbContext.PermissionGrants.RemoveRange(orphanedGrants);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Removed {Count} orphaned permission grant(s).", orphanedGrants.Count);
    }
}
