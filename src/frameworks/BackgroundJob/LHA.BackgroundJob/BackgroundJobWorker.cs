using LHA.BackgroundWorker;
using LHA.DistributedLocking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.BackgroundJob;

/// <summary>
/// Periodic background worker that polls <see cref="IBackgroundJobStore"/> for waiting jobs
/// and executes them using <see cref="IBackgroundJobExecuter"/>.
/// Uses a distributed lock to ensure only one node processes jobs at a time.
/// </summary>
public sealed class BackgroundJobWorker : PeriodicBackgroundWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<BackgroundJobOptions> _jobOptions;
    private readonly IOptions<BackgroundJobWorkerOptions> _workerOptions;
    private readonly IDistributedLock _distributedLock;
    private readonly TimeProvider _timeProvider;

    public BackgroundJobWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<BackgroundJobOptions> jobOptions,
        IOptions<BackgroundJobWorkerOptions> workerOptions,
        IDistributedLock distributedLock,
        ILogger<BackgroundJobWorker> logger,
        TimeProvider? timeProvider = null)
        : base(TimeSpan.FromMilliseconds(workerOptions.Value.JobPollPeriodMs), logger)
    {
        _scopeFactory = scopeFactory;
        _jobOptions = jobOptions;
        _workerOptions = workerOptions;
        _distributedLock = distributedLock;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        if (!_jobOptions.Value.IsJobExecutionEnabled)
            return;

        var opts = _workerOptions.Value;

        await using var handle = await _distributedLock.TryAcquireAsync(
            opts.DistributedLockName, cancellationToken: stoppingToken);

        if (handle is null)
        {
            // Another node holds the lock — back off
            try
            {
                await Task.Delay(opts.JobPollPeriodMs * opts.LockWaitMultiplier, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
            }
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IBackgroundJobStore>();
        var executer = scope.ServiceProvider.GetRequiredService<IBackgroundJobExecuter>();
        var serializer = scope.ServiceProvider.GetRequiredService<IBackgroundJobSerializer>();

        var waitingJobs = await store.GetWaitingJobsAsync(
            opts.ApplicationName, opts.MaxJobFetchCount, stoppingToken);

        if (waitingJobs.Count == 0)
            return;

        foreach (var jobInfo in waitingJobs)
        {
            if (stoppingToken.IsCancellationRequested) break;

            jobInfo.TryCount++;
            jobInfo.LastTryTime = _timeProvider.GetUtcNow();

            try
            {
                var jobConfig = _jobOptions.Value.GetJob(jobInfo.JobName);
                var jobArgs = serializer.Deserialize(jobInfo.JobArgs, jobConfig.ArgsType);

                var context = new JobExecutionContext
                {
                    ServiceProvider = scope.ServiceProvider,
                    JobType = jobConfig.JobType,
                    ArgsType = jobConfig.ArgsType,
                    JobArgs = jobArgs,
                    CancellationToken = stoppingToken
                };

                await executer.ExecuteAsync(context);
                await store.DeleteAsync(jobInfo.Id, stoppingToken);
            }
            catch (BackgroundJobExecutionException)
            {
                var nextTryTime = CalculateNextTryTime(jobInfo);
                if (nextTryTime.HasValue)
                {
                    jobInfo.NextTryTime = nextTryTime.Value;
                }
                else
                {
                    jobInfo.IsAbandoned = true;
                }

                await TryUpdateAsync(store, jobInfo, stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error processing background job {JobId}.", jobInfo.Id);
                jobInfo.IsAbandoned = true;
                await TryUpdateAsync(store, jobInfo, stoppingToken);
            }
        }
    }

    private DateTimeOffset? CalculateNextTryTime(BackgroundJobEntity jobInfo)
    {
        var opts = _workerOptions.Value;
        var nextWaitSeconds = opts.FirstRetryWaitSeconds *
                              Math.Pow(opts.RetryWaitFactor, jobInfo.TryCount - 1);

        var nextTryTime = (jobInfo.LastTryTime ?? _timeProvider.GetUtcNow())
            .AddSeconds(nextWaitSeconds);

        // Abandon if the job has been around too long
        if (nextTryTime.Subtract(jobInfo.CreationTime).TotalSeconds > opts.JobTimeoutSeconds)
            return null;

        return nextTryTime;
    }

    private async Task TryUpdateAsync(
        IBackgroundJobStore store,
        BackgroundJobEntity jobInfo,
        CancellationToken cancellationToken)
    {
        try
        {
            await store.UpdateAsync(jobInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to update background job {JobId}.", jobInfo.Id);
        }
    }
}
