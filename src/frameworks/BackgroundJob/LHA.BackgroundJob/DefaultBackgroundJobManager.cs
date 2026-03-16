using Microsoft.Extensions.Options;

namespace LHA.BackgroundJob;

/// <summary>
/// Default store-based implementation of <see cref="IBackgroundJobManager"/>.
/// Serializes job args → persists to <see cref="IBackgroundJobStore"/> →
/// <see cref="BackgroundJobWorker"/> polls and executes.
/// </summary>
public sealed class DefaultBackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobStore _store;
    private readonly IBackgroundJobSerializer _serializer;
    private readonly IOptions<BackgroundJobOptions> _jobOptions;
    private readonly IOptions<BackgroundJobWorkerOptions> _workerOptions;
    private readonly TimeProvider _timeProvider;

    public DefaultBackgroundJobManager(
        IBackgroundJobStore store,
        IBackgroundJobSerializer serializer,
        IOptions<BackgroundJobOptions> jobOptions,
        IOptions<BackgroundJobWorkerOptions> workerOptions,
        TimeProvider? timeProvider = null)
    {
        _store = store;
        _serializer = serializer;
        _jobOptions = jobOptions;
        _workerOptions = workerOptions;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    public async Task<string> EnqueueAsync<TArgs>(
        TArgs args,
        BackgroundJobPriority priority = BackgroundJobPriority.Normal,
        TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(args);

        var jobName = _jobOptions.Value.JobNameResolver(typeof(TArgs));
        var now = _timeProvider.GetUtcNow();

        var jobInfo = new BackgroundJobEntity
        {
            Id = Guid.CreateVersion7(),
            ApplicationName = _workerOptions.Value.ApplicationName,
            JobName = jobName,
            JobArgs = _serializer.Serialize(args),
            Priority = priority,
            CreationTime = now,
            NextTryTime = delay.HasValue ? now.Add(delay.Value) : now
        };

        await _store.InsertAsync(jobInfo);

        return jobInfo.Id.ToString();
    }
}
