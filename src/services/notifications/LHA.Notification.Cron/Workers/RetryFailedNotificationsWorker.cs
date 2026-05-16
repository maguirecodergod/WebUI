using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Retries notifications that failed during delivery, respecting the configured
/// maximum retry count and exponential backoff strategy.
/// </summary>
public sealed class RetryFailedNotificationsWorker : HangfirePeriodicBackgroundWorker
{
    // Run every 2 minutes
    protected override string CronExpression => "*/2 * * * *";

    public RetryFailedNotificationsWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<RetryFailedNotificationsWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting retry of failed notifications...");

        // TODO: Resolve INotificationRepository from a scope,
        // query notifications where Status = Failed and RetryCount < MaxRetries
        // and NextRetryAt <= UtcNow, re-enqueue them through the channel pipeline.

        await Task.CompletedTask;
    }
}
