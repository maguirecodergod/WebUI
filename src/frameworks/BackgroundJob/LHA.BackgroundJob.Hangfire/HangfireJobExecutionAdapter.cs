using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.BackgroundJob.Hangfire;

/// <summary>
/// Adapter that bridges Hangfire's job execution to the LHA background job system.
/// Hangfire serializes method call arguments — this class is the target invoked by Hangfire.
/// It deserializes the args, resolves the job from DI, and delegates to
/// <see cref="IBackgroundJobExecuter"/>.
/// </summary>
/// <typeparam name="TArgs">The background job arguments type.</typeparam>
public sealed class HangfireJobExecutionAdapter<TArgs>
{
    private readonly IBackgroundJobExecuter _executer;
    private readonly IBackgroundJobSerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundJobOptions _options;
    private readonly ILogger<HangfireJobExecutionAdapter<TArgs>> _logger;

    public HangfireJobExecutionAdapter(
        IBackgroundJobExecuter executer,
        IBackgroundJobSerializer serializer,
        IServiceScopeFactory scopeFactory,
        IOptions<BackgroundJobOptions> options,
        ILogger<HangfireJobExecutionAdapter<TArgs>> logger)
    {
        _executer = executer;
        _serializer = serializer;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Entry point invoked by Hangfire. Deserializes the job args and delegates to the executer.
    /// </summary>
    /// <param name="serializedArgs">JSON-serialized job arguments.</param>
    /// <param name="cancellationToken">Hangfire cancellation token.</param>
    [global::Hangfire.AutomaticRetry(Attempts = 0)] // Retry handled by LHA, not Hangfire
    public async Task ExecuteAsync(string serializedArgs, CancellationToken cancellationToken)
    {
        if (!_options.IsJobExecutionEnabled)
        {
            _logger.LogWarning(
                "Background job execution is disabled. Skipping job with args type {ArgsType}.",
                typeof(TArgs).Name);
            return;
        }

        var argsType = typeof(TArgs);
        var config = _options.GetJob(argsType);
        var args = _serializer.Deserialize(serializedArgs, argsType);

        await using var scope = _scopeFactory.CreateAsyncScope();

        var context = new JobExecutionContext
        {
            ServiceProvider = scope.ServiceProvider,
            JobType = config.JobType,
            ArgsType = argsType,
            JobArgs = args,
            CancellationToken = cancellationToken
        };

        _logger.LogDebug(
            "Hangfire executing background job {JobType} with args type {ArgsType}.",
            config.JobType.Name, argsType.Name);

        await _executer.ExecuteAsync(context);
    }
}
