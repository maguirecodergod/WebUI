namespace LHA.BackgroundJob;

/// <summary>
/// Persistence store for background jobs.
/// Implement this interface with a database-backed store for production use.
/// The default <see cref="InMemoryBackgroundJobStore"/> is suitable for testing only.
/// </summary>
public interface IBackgroundJobStore
{
    /// <summary>Finds a job by its ID.</summary>
    Task<BackgroundJobEntity?> FindAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>Inserts a new job.</summary>
    Task InsertAsync(BackgroundJobEntity jobInfo, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing job (e.g. increment try count, mark abandoned).</summary>
    Task UpdateAsync(BackgroundJobEntity jobInfo, CancellationToken cancellationToken = default);

    /// <summary>Deletes a completed job.</summary>
    Task DeleteAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets waiting jobs eligible for execution.
    /// Conditions: matching applicationName, not abandoned, NextTryTime &lt;= now.
    /// Ordered by: Priority DESC, TryCount ASC, NextTryTime ASC.
    /// </summary>
    Task<List<BackgroundJobEntity>> GetWaitingJobsAsync(
        string? applicationName,
        int maxResultCount,
        CancellationToken cancellationToken = default);
}
