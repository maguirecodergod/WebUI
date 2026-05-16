using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Picks up notifications that were scheduled for future delivery and dispatches
/// them through the standard channel pipeline when their send time arrives.
/// </summary>
public sealed class ProcessScheduledNotificationsWorker : HangfirePeriodicBackgroundWorker
{
    // Run every minute to ensure near-real-time scheduled delivery
    protected override string CronExpression => "* * * * *";

    public ProcessScheduledNotificationsWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<ProcessScheduledNotificationsWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogDebug("Checking for scheduled notifications ready to send...");

        // TODO: Resolve INotificationRepository from a scope,
        // query notifications where ScheduledAt <= UtcNow and Status = Scheduled,
        // enqueue each through the channel pipeline for delivery.

        await Task.CompletedTask;
    }
}
