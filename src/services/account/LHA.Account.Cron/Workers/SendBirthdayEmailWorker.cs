using Hangfire;
using LHA.BackgroundWorker.Hangfire;

namespace LHA.Account.Cron.Workers;

public class SendBirthdayEmailWorker : HangfirePeriodicBackgroundWorker
{
    // Run every day at 08:00 AM
    protected override string CronExpression => "0 8 * * ?";

    public SendBirthdayEmailWorker(
        IRecurringJobManager recurringJobManager,
        ILogger<SendBirthdayEmailWorker> logger)
        : base(recurringJobManager, logger)
    {
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Start sending birthday emails to users...");

        // TODO: Implement birthday email sending logic here.
        // For example, resolve a scoped service and execute it.

        await Task.CompletedTask;
    }
}
