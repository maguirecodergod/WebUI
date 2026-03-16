using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.BackgroundJob;

/// <summary>
/// Default implementation of <see cref="IBackgroundJobExecuter"/>.
/// Resolves the job from DI and invokes <see cref="IBackgroundJob{TArgs}.ExecuteAsync"/>.
/// </summary>
public sealed class BackgroundJobExecuter : IBackgroundJobExecuter
{
    private readonly ILogger<BackgroundJobExecuter> _logger;

    public BackgroundJobExecuter(ILogger<BackgroundJobExecuter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(JobExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var job = context.ServiceProvider.GetRequiredService(context.JobType);

        var executeMethod = typeof(IBackgroundJob<>)
            .MakeGenericType(context.ArgsType)
            .GetMethod(nameof(IBackgroundJob<object>.ExecuteAsync))
            ?? throw new InvalidOperationException(
                $"Cannot find ExecuteAsync method on {typeof(IBackgroundJob<>).MakeGenericType(context.ArgsType).FullName}.");

        _logger.LogDebug("Executing background job {JobType} with args {ArgsType}.",
            context.JobType.Name, context.ArgsType.Name);

        try
        {
            var task = (Task)executeMethod.Invoke(job, [context.JobArgs, context.CancellationToken])!;
            await task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background job {JobType} failed.", context.JobType.Name);

            throw new BackgroundJobExecutionException(
                $"Background job execution failed. Job type: {context.JobType.Name}.", ex)
            {
                JobType = context.JobType.AssemblyQualifiedName,
                JobArgs = context.JobArgs
            };
        }
    }
}
