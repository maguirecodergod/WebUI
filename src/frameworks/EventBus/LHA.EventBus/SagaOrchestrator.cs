using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.EventBus;

/// <summary>
/// Default <see cref="ISagaOrchestrator"/> that executes saga steps sequentially
/// and compensates completed steps in reverse order on failure.
/// <para>
/// Supports distributed transaction avoidance by treating each step as an
/// independent operation with a compensating action for rollback.
/// </para>
/// </summary>
public sealed class SagaOrchestrator : ISagaOrchestrator
{
    private readonly ILogger<SagaOrchestrator> _logger;

    public SagaOrchestrator(ILogger<SagaOrchestrator>? logger = null)
    {
        _logger = logger ?? NullLogger<SagaOrchestrator>.Instance;
    }

    /// <inheritdoc />
    public async Task<SagaExecutionResult> ExecuteAsync<TState>(
        ISaga<TState> saga,
        TState? initialState = null,
        CancellationToken cancellationToken = default)
        where TState : class, new()
    {
        ArgumentNullException.ThrowIfNull(saga);

        var state = initialState ?? new TState();
        var completedSteps = new Stack<ISagaStep<TState>>();
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Starting saga '{SagaName}' with {StepCount} steps.",
            saga.SagaName, saga.Steps.Count);

        foreach (var step in saga.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Executing saga step '{StepName}'.", step.StepName);

            var result = await step.ExecuteAsync(state, cancellationToken);

            if (result.Status == CSagaStepStatus.Completed)
            {
                completedSteps.Push(step);
                _logger.LogDebug("Saga step '{StepName}' completed.", step.StepName);
                continue;
            }

            // Step failed — compensate
            _logger.LogWarning("Saga step '{StepName}' failed: {Reason}. Starting compensation.",
                step.StepName, result.FailureReason);

            var (compensated, failedCompensations) = await CompensateAsync(completedSteps, state, cancellationToken);

            sw.Stop();
            return new SagaExecutionResult
            {
                IsSuccess = false,
                FailedStep = step.StepName,
                FailureReason = result.FailureReason,
                CompensatedSteps = compensated,
                FailedCompensations = failedCompensations,
                Elapsed = sw.Elapsed
            };
        }

        sw.Stop();
        _logger.LogInformation("Saga '{SagaName}' completed successfully in {Elapsed}ms.",
            saga.SagaName, sw.ElapsedMilliseconds);

        return SagaExecutionResult.Success(sw.Elapsed);
    }

    private async Task<(List<string> compensated, List<string> failed)> CompensateAsync<TState>(
        Stack<ISagaStep<TState>> completedSteps,
        TState state,
        CancellationToken cancellationToken)
        where TState : class
    {
        var compensated = new List<string>();
        var failed = new List<string>();

        while (completedSteps.TryPop(out var step))
        {
            try
            {
                _logger.LogDebug("Compensating saga step '{StepName}'.", step.StepName);
                var result = await step.CompensateAsync(state, cancellationToken);

                if (result.Status == CSagaStepStatus.Compensated || result.Status == CSagaStepStatus.Completed)
                {
                    compensated.Add(step.StepName);
                    _logger.LogDebug("Compensation of '{StepName}' succeeded.", step.StepName);
                }
                else
                {
                    failed.Add(step.StepName);
                    _logger.LogError("Compensation of '{StepName}' failed: {Reason}. Manual intervention required.",
                        step.StepName, result.FailureReason);
                }
            }
            catch (Exception ex)
            {
                failed.Add(step.StepName);
                _logger.LogError(ex, "Compensation of '{StepName}' threw an exception. Manual intervention required.",
                    step.StepName);
            }
        }

        return (compensated, failed);
    }
}
