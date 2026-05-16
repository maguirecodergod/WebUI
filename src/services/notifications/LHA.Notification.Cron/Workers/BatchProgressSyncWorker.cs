using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Synchronizes batch notification progress by reconciling
/// the actual delivery status of individual notifications
/// against the parent <c>NotificationBatch</c> aggregate.
/// </summary>
public sealed class BatchProgressSyncWorker : HangfirePeriodicBackgroundWorker
{
    // Run every 5 minutes
    protected override string CronExpression => "*/5 * * * *";

    public BatchProgressSyncWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<BatchProgressSyncWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting batch progress synchronization...");

        // TODO: Resolve INotificationBatchRepository from a scope,
        // query incomplete batches, recalculate progress (sent/failed/pending counts),
        // and mark batches as Completed when all items have been processed.

        await Task.CompletedTask;
    }
}
