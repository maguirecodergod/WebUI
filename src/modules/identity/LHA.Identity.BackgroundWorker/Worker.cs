using LHA.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.BackgroundWorker;

/// <summary>
/// Periodically removes expired user tokens (e.g., refresh tokens).
/// </summary>
public sealed class ExpiredTokenCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredTokenCleanupWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ExpiredTokenCleanupWorker started. Interval={Interval}", Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error during expired token cleanup.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var now = DateTimeOffset.UtcNow;
        var deleted = await dbContext.UserTokens
            .Where(t => t.ExpiresAt != null && t.ExpiresAt < now)
            .ExecuteDeleteAsync(ct);

        if (deleted > 0)
            logger.LogInformation("Purged {Count} expired user token(s).", deleted);
    }
}

/// <summary>
/// Periodically purges old security logs beyond the retention period.
/// </summary>
public sealed class SecurityLogCleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<SecurityLogCleanupWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(12);
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(90);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "SecurityLogCleanupWorker started. Interval={Interval}, Retention={Retention}",
            Interval, RetentionPeriod);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error during security log cleanup.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var cutoff = DateTimeOffset.UtcNow - RetentionPeriod;
        var deleted = await dbContext.SecurityLogs
            .Where(sl => sl.CreationTime < cutoff)
            .ExecuteDeleteAsync(ct);

        if (deleted > 0)
            logger.LogInformation("Purged {Count} old security log(s) older than {Retention}.",
                deleted, RetentionPeriod);
    }
}
