using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.BackgroundJob.Hangfire;

/// <summary>
/// <see cref="IBackgroundJobManager"/> implementation that delegates to Hangfire.
/// Jobs are enqueued via Hangfire's <c>BackgroundJob.Enqueue</c> /
/// <c>BackgroundJob.Schedule</c> and executed through <see cref="HangfireJobExecutionAdapter{TArgs}"/>.
/// </summary>
public sealed class HangfireBackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobSerializer _serializer;
    private readonly BackgroundJobOptions _options;
    private readonly ILogger<HangfireBackgroundJobManager> _logger;

    public HangfireBackgroundJobManager(
        IBackgroundJobSerializer serializer,
        IOptions<BackgroundJobOptions> options,
        ILogger<HangfireBackgroundJobManager> logger)
    {
        _serializer = serializer;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string> EnqueueAsync<TArgs>(
        TArgs args,
        BackgroundJobPriority priority = BackgroundJobPriority.Normal,
        TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(args);

        // Validate that the job is registered
        var config = _options.GetJob(typeof(TArgs));
        var serializedArgs = _serializer.Serialize(args);
        var queue = MapPriorityToQueue(priority);

        string hangfireJobId;

        if (delay.HasValue && delay.Value > TimeSpan.Zero)
        {
            hangfireJobId = global::Hangfire.BackgroundJob.Schedule<HangfireJobExecutionAdapter<TArgs>>(
                queue,
                adapter => adapter.ExecuteAsync(serializedArgs, CancellationToken.None),
                delay.Value);

            _logger.LogInformation(
                "Scheduled background job {JobName} with delay {Delay}. Hangfire ID: {HangfireJobId}.",
                config.JobName, delay.Value, hangfireJobId);
        }
        else
        {
            hangfireJobId = global::Hangfire.BackgroundJob.Enqueue<HangfireJobExecutionAdapter<TArgs>>(
                queue,
                adapter => adapter.ExecuteAsync(serializedArgs, CancellationToken.None));

            _logger.LogInformation(
                "Enqueued background job {JobName}. Hangfire ID: {HangfireJobId}.",
                config.JobName, hangfireJobId);
        }

        return Task.FromResult(hangfireJobId);
    }

    /// <summary>
    /// Maps <see cref="BackgroundJobPriority"/> to a Hangfire queue name.
    /// Higher priority jobs go to "critical" / "high" queues.
    /// </summary>
    private static string MapPriorityToQueue(BackgroundJobPriority priority) => priority switch
    {
        BackgroundJobPriority.High => "critical",
        BackgroundJobPriority.AboveNormal => "high",
        BackgroundJobPriority.Normal => "default",
        BackgroundJobPriority.BelowNormal => "low",
        BackgroundJobPriority.Low => "low",
        _ => "default"
    };
}
