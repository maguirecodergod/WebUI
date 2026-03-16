using System.Collections.Concurrent;

namespace LHA.BackgroundJob;

/// <summary>
/// In-memory implementation of <see cref="IBackgroundJobStore"/>.
/// Suitable for testing and single-node development only.
/// For production, use a database-backed store (e.g. EF Core).
/// </summary>
public sealed class InMemoryBackgroundJobStore : IBackgroundJobStore
{
    private readonly ConcurrentDictionary<Guid, BackgroundJobEntity> _jobs = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryBackgroundJobStore(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    public Task<BackgroundJobEntity?> FindAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _jobs.TryGetValue(jobId, out var job);
        return Task.FromResult(job);
    }

    /// <inheritdoc />
    public Task InsertAsync(BackgroundJobEntity jobInfo, CancellationToken cancellationToken = default)
    {
        _jobs[jobInfo.Id] = jobInfo;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateAsync(BackgroundJobEntity jobInfo, CancellationToken cancellationToken = default)
    {
        if (jobInfo.IsAbandoned)
        {
            _jobs.TryRemove(jobInfo.Id, out _);
        }
        else
        {
            _jobs[jobInfo.Id] = jobInfo;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _jobs.TryRemove(jobId, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<List<BackgroundJobEntity>> GetWaitingJobsAsync(
        string? applicationName,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow();

        var waitingJobs = _jobs.Values
            .Where(j => j.ApplicationName == applicationName)
            .Where(j => !j.IsAbandoned && j.NextTryTime <= now)
            .OrderByDescending(j => j.Priority)
            .ThenBy(j => j.TryCount)
            .ThenBy(j => j.NextTryTime)
            .Take(maxResultCount)
            .ToList();

        return Task.FromResult(waitingJobs);
    }
}
