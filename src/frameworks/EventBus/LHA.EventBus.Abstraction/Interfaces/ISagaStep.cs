namespace LHA.EventBus;

/// <summary>
/// Defines a single step in a saga with execute and compensate actions.
/// <para>
/// Steps are executed in order. If any step fails, previously completed
/// steps are compensated in reverse order (backward recovery).
/// </para>
/// </summary>
/// <typeparam name="TState">The saga state type that flows between steps.</typeparam>
public interface ISagaStep<TState> where TState : class
{
    /// <summary>Human-readable step name for logging and diagnostics.</summary>
    string StepName { get; }

    /// <summary>
    /// Executes the forward action of this step.
    /// </summary>
    /// <param name="state">The current saga state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The execution result.</returns>
    Task<SagaStepResult> ExecuteAsync(TState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compensates (rolls back) this step when a later step fails.
    /// </summary>
    /// <param name="state">The current saga state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The compensation result.</returns>
    Task<SagaStepResult> CompensateAsync(TState state, CancellationToken cancellationToken = default);
}
