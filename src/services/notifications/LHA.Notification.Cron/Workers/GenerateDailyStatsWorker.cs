using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Aggregates notification delivery metrics for the previous day and persists
/// them as daily statistics for dashboards and reporting.
/// </summary>
public sealed class GenerateDailyStatsWorker : HangfirePeriodicBackgroundWorker
{
    // Run daily at 00:30 AM UTC (after midnight to capture the full day)
    protected override string CronExpression => "30 0 * * *";

    public GenerateDailyStatsWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<GenerateDailyStatsWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting daily notification stats generation...");

        // TODO: Resolve INotificationRepository from a scope,
        // aggregate counts by channel, status, and tenant for the previous day,
        // persist results (e.g. NotificationDailyStats collection).

        await Task.CompletedTask;
    }
}
