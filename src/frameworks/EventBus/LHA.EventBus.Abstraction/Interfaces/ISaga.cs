namespace LHA.EventBus;

/// <summary>
/// Defines a saga — a long-running, distributed process composed of ordered steps
/// with compensating actions for failure recovery.
/// <para>
/// Designed for distributed transaction avoidance: each step is an independent
/// operation that can be compensated rather than rolled back in a distributed transaction.
/// </para>
/// </summary>
/// <typeparam name="TState">
/// State object that flows through the saga steps. Must have a parameterless constructor.
/// </typeparam>
public interface ISaga<TState> where TState : class, new()
{
    /// <summary>Unique saga name for logging and persistence.</summary>
    string SagaName { get; }

    /// <summary>Ordered list of steps to execute.</summary>
    IReadOnlyList<ISagaStep<TState>> Steps { get; }
}

/// <summary>
/// Outcome of a complete saga execution.
/// </summary>
public sealed class SagaExecutionResult
{
    /// <summary>Whether all steps completed successfully.</summary>
    public required bool IsSuccess { get; init; }

    /// <summary>The step that failed (if any).</summary>
    public string? FailedStep { get; init; }

    /// <summary>Failure reason from the failed step.</summary>
    public string? FailureReason { get; init; }

    /// <summary>Steps that were successfully compensated during rollback.</summary>
    public IReadOnlyList<string> CompensatedSteps { get; init; } = [];

    /// <summary>Steps that failed compensation (requires manual intervention).</summary>
    public IReadOnlyList<string> FailedCompensations { get; init; } = [];

    /// <summary>Total elapsed time for saga execution.</summary>
    public TimeSpan Elapsed { get; init; }

    /// <summary>Creates a successful saga result.</summary>
    public static SagaExecutionResult Success(TimeSpan elapsed) =>
        new() { IsSuccess = true, Elapsed = elapsed };
}

/// <summary>
/// Orchestrates saga execution with forward steps and backward compensation.
/// </summary>
public interface ISagaOrchestrator
{
    /// <summary>
    /// Executes the saga: runs steps in order, compensates on failure.
    /// </summary>
    /// <typeparam name="TState">Saga state type.</typeparam>
    /// <param name="saga">The saga definition.</param>
    /// <param name="initialState">Optional initial state (creates new if null).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The saga execution result.</returns>
    Task<SagaExecutionResult> ExecuteAsync<TState>(
        ISaga<TState> saga,
        TState? initialState = null,
        CancellationToken cancellationToken = default)
        where TState : class, new();
}
