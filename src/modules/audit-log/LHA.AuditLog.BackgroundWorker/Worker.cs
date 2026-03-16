using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.AuditLog.BackgroundWorker;

/// <summary>
/// Periodic worker that permanently deletes audit logs older than the retention period.
/// <para>
/// Default: runs every 24 hours, deletes logs older than <see cref="AuditLogConsts.DefaultRetentionDays"/> days.
/// </para>
/// </summary>
public sealed class AuditLogCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AuditLogCleanupWorker> logger) : BackgroundService
{
    /// <summary>How often to run (default: every 24 hours).</summary>
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    /// <summary>How long to retain audit logs before hard-delete.</summary>
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(AuditLogConsts.DefaultRetentionDays);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "AuditLogCleanupWorker started. Interval={Interval}, Retention={Retention}",
            Interval, RetentionPeriod);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error during audit log cleanup.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        var cutoff = DateTimeOffset.UtcNow - RetentionPeriod;

        logger.LogInformation("Starting audit log cleanup. Deleting logs older than {CutoffTime}...", cutoff);

        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

        var deletedCount = await repository.DeleteOlderThanAsync(cutoff, cancellationToken);

        logger.LogInformation("Audit log cleanup complete. Deleted {Count} records.", deletedCount);
    }
}
