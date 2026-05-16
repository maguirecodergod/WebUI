using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Reconciles the cached unread notification counts in Redis with the actual
/// counts in the database, correcting any drift caused by missed events or cache eviction.
/// </summary>
public sealed class UnreadCountReconciliationWorker : HangfirePeriodicBackgroundWorker
{
    // Run every 15 minutes
    protected override string CronExpression => "*/15 * * * *";

    public UnreadCountReconciliationWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<UnreadCountReconciliationWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting unread count reconciliation...");

        // TODO: Resolve INotificationRepository and IUnreadCountCache from a scope,
        // query actual unread counts per user from the database,
        // compare with Redis cached values and overwrite any mismatches.

        await Task.CompletedTask;
    }
}
