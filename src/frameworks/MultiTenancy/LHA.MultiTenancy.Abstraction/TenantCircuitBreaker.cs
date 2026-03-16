using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace LHA.MultiTenancy;

/// <summary>
/// Sliding-window circuit breaker tracked per-tenant using <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// Designed for millions of tenants — entries are lightweight and lazily created.
/// </summary>
internal sealed class TenantCircuitBreaker : ITenantCircuitBreaker
{
    private readonly ConcurrentDictionary<Guid, TenantCircuitEntry> _circuits = new();
    private readonly TenantCircuitBreakerOptions _options;

    public TenantCircuitBreaker(IOptions<TenantCircuitBreakerOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public CTenantCircuitStateType GetState(Guid tenantId)
    {
        if (!_options.IsEnabled) return CTenantCircuitStateType.Closed;
        return _circuits.TryGetValue(tenantId, out var entry) ? entry.GetState(_options) : CTenantCircuitStateType.Closed;
    }

    /// <inheritdoc />
    public bool AllowRequest(Guid tenantId)
    {
        if (!_options.IsEnabled) return true;

        if (!_circuits.TryGetValue(tenantId, out var entry)) return true;

        var state = entry.GetState(_options);
        return state switch
        {
            CTenantCircuitStateType.Closed => true,
            CTenantCircuitStateType.HalfOpen => entry.TryAllowProbe(_options.HalfOpenMaxProbes),
            CTenantCircuitStateType.Open => false,
            _ => true
        };
    }

    /// <inheritdoc />
    public void RecordSuccess(Guid tenantId)
    {
        if (!_options.IsEnabled) return;

        if (_circuits.TryGetValue(tenantId, out var entry))
        {
            entry.RecordSuccess(_options);
        }
    }

    /// <inheritdoc />
    public void RecordFailure(Guid tenantId)
    {
        if (!_options.IsEnabled) return;

        var entry = _circuits.GetOrAdd(tenantId, _ => new TenantCircuitEntry());
        entry.RecordFailure();
    }

    /// <inheritdoc />
    public void Reset(Guid tenantId)
    {
        _circuits.TryRemove(tenantId, out _);
    }

    /// <summary>
    /// Internal per-tenant circuit state. Thread-safe via interlocked operations.
    /// </summary>
    private sealed class TenantCircuitEntry
    {
        private readonly ConcurrentQueue<DateTime> _failureTimestamps = new();
        private volatile int _consecutiveSuccesses;
        private volatile int _probeCount;
        private DateTime _openedAtUtc;
        private volatile bool _isOpen;

        public CTenantCircuitStateType GetState(TenantCircuitBreakerOptions options)
        {
            if (!_isOpen)
            {
                return CTenantCircuitStateType.Closed;
            }

            var elapsed = DateTime.UtcNow - _openedAtUtc;
            if (elapsed >= options.OpenDuration)
            {
                return CTenantCircuitStateType.HalfOpen;
            }

            return CTenantCircuitStateType.Open;
        }

        public void RecordFailure()
        {
            var now = DateTime.UtcNow;
            _failureTimestamps.Enqueue(now);
            _consecutiveSuccesses = 0;
            _probeCount = 0;
        }

        public void RecordSuccess(TenantCircuitBreakerOptions options)
        {
            if (_isOpen)
            {
                Interlocked.Increment(ref _consecutiveSuccesses);
                if (_consecutiveSuccesses >= options.SuccessThresholdToClose)
                {
                    // Close circuit
                    _isOpen = false;
                    _consecutiveSuccesses = 0;
                    _probeCount = 0;
                    // Drain failure queue
                    while (_failureTimestamps.TryDequeue(out _)) { }
                }
            }
            else
            {
                // Remove old failures outside window
                PruneOldFailures(options);
            }
        }

        public bool TryAllowProbe(int maxProbes)
        {
            var current = Interlocked.Increment(ref _probeCount);
            return current <= maxProbes;
        }

        private void PruneOldFailures(TenantCircuitBreakerOptions options)
        {
            var cutoff = DateTime.UtcNow - options.FailureWindow;
            while (_failureTimestamps.TryPeek(out var ts) && ts < cutoff)
            {
                _failureTimestamps.TryDequeue(out _);
            }

            if (_failureTimestamps.Count >= options.FailureThreshold)
            {
                _isOpen = true;
                _openedAtUtc = DateTime.UtcNow;
                _probeCount = 0;
                _consecutiveSuccesses = 0;
            }
        }
    }
}
