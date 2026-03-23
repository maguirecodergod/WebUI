using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Circuit breaker for the audit dispatch pipeline.
/// <para>
/// After <see cref="AuditPipelineOptions.CircuitBreakerThreshold"/> consecutive
/// failures, the circuit opens and the pipeline switches to the fallback dispatcher.
/// After <see cref="AuditPipelineOptions.CircuitBreakerRecoveryMs"/> the circuit
/// enters half-open state, allowing one probe dispatch.
/// </para>
/// </summary>
internal sealed class AuditPipelineCircuitBreaker
{
    private readonly AuditPipelineOptions _options;
    private int _consecutiveFailures;
    private DateTimeOffset _openedAt;
    private CircuitState _state = CircuitState.Closed;
    private readonly object _lock = new();

    public AuditPipelineCircuitBreaker(IOptions<AuditPipelineOptions> options)
    {
        _options = options.Value;
    }

    public bool IsOpen
    {
        get
        {
            lock (_lock)
            {
                if (_state == CircuitState.Closed)
                    return false;

                // Check if recovery period has elapsed
                var elapsed = DateTimeOffset.UtcNow - _openedAt;
                if (elapsed.TotalMilliseconds >= _options.CircuitBreakerRecoveryMs)
                {
                    _state = CircuitState.HalfOpen;
                    return false; // Allow probe
                }

                return true;
            }
        }
    }

    public void RecordSuccess()
    {
        lock (_lock)
        {
            _consecutiveFailures = 0;
            _state = CircuitState.Closed;
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= _options.CircuitBreakerThreshold)
            {
                _state = CircuitState.Open;
                _openedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    private enum CircuitState
    {
        Closed,
        Open,
        HalfOpen
    }
}
