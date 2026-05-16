using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Transitions notifications that have passed their expiration date
/// to <c>Expired</c> status so they are no longer shown to end users.
/// </summary>
public sealed class ExpireOldNotificationsWorker : HangfirePeriodicBackgroundWorker
{
    // Run every hour
    protected override string CronExpression => "0 * * * *";

    public ExpireOldNotificationsWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<ExpireOldNotificationsWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting expiration of old notifications...");

        // TODO: Resolve INotificationRepository from a scope,
        // query notifications where ExpiresAt <= UtcNow and Status != Expired,
        // bulk-update their Status to Expired.

        await Task.CompletedTask;
    }
}
