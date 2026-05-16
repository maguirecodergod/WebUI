using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Notification.Cron.Workers;

/// <summary>
/// Marks push-notification devices as inactive when they haven't communicated
/// with the platform within the configured inactivity threshold.
/// This prevents wasted push provider calls to stale device tokens.
/// </summary>
public sealed class DeactivateInactiveDevicesWorker : HangfirePeriodicBackgroundWorker
{
    // Run daily at 03:00 AM UTC
    protected override string CronExpression => "0 3 * * *";

    public DeactivateInactiveDevicesWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<DeactivateInactiveDevicesWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting deactivation of inactive devices...");

        // TODO: Resolve IDeviceRepository from a scope,
        // query devices where LastActiveAt < (UtcNow - InactivityThreshold),
        // set IsActive = false and persist changes.

        await Task.CompletedTask;
    }
}
