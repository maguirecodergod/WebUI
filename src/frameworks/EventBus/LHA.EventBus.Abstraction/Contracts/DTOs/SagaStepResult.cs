namespace LHA.EventBus;

/// <summary>
/// Result of a single <see cref="ISagaStep{TState}"/> execution.
/// </summary>
public sealed class SagaStepResult
{
    /// <summary>The outcome status.</summary>
    public required CSagaStepStatus Status { get; init; }

    /// <summary>Reason for failure (when <see cref="Status"/> is <see cref="CSagaStepStatus.Failed"/>).</summary>
    public string? FailureReason { get; init; }

    /// <summary>Creates a successful result.</summary>
    public static SagaStepResult Success() => new() { Status = CSagaStepStatus.Completed };

    /// <summary>Creates a failed result with a reason.</summary>
    public static SagaStepResult Failure(string reason) => new() { Status = CSagaStepStatus.Failed, FailureReason = reason };
}
