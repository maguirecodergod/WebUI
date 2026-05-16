using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Removes or archives delivered notifications that exceed the configured retention period,
/// keeping the database lean and query performance optimal.
/// </summary>
public sealed class CleanupDeliveredNotificationWorker : HangfirePeriodicBackgroundWorker
{
    // Run daily at 02:00 AM UTC
    protected override string CronExpression => "0 2 * * *";

    public CleanupDeliveredNotificationWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<CleanupDeliveredNotificationWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting cleanup of delivered notifications...");

        // TODO: Resolve INotificationRepository from a scope,
        // delete or soft-delete notifications with Status = Delivered
        // that are older than the configured retention (e.g. 90 days).

        await Task.CompletedTask;
    }
}
