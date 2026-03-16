namespace LHA.BackgroundJob;

/// <summary>
/// Executes a background job given its context.
/// The default implementation resolves the job from DI and invokes
/// <see cref="IBackgroundJob{TArgs}.ExecuteAsync"/>.
/// </summary>
public interface IBackgroundJobExecuter
{
    /// <summary>
    /// Executes the job described by <paramref name="context"/>.
    /// </summary>
    Task ExecuteAsync(JobExecutionContext context);
}
