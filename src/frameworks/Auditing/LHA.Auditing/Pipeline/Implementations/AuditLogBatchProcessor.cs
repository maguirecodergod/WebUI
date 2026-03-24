using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Background service that reads batches from <see cref="IAuditLogBuffer"/>
/// and dispatches them via <see cref="IAuditLogDispatcher"/>.
/// <para>
/// Batches by both size and time window. Includes circuit breaker
/// for resilient dispatch with automatic fallback.
/// </para>
/// </summary>
internal sealed class AuditLogBatchProcessor : BackgroundService
{
    private readonly IAuditLogBuffer _buffer;
    private readonly IAuditLogDispatcher _primaryDispatcher;
    private readonly IAuditLogDispatcher _fallbackDispatcher;
    private readonly AuditPipelineCircuitBreaker _circuitBreaker;
    private readonly AuditPipelineMetrics _metrics;
    private readonly AuditPipelineOptions _options;
    private readonly ILogger<AuditLogBatchProcessor> _logger;

    public AuditLogBatchProcessor(
        IAuditLogBuffer buffer,
        IAuditLogDispatcher primaryDispatcher,
        AuditPipelineCircuitBreaker circuitBreaker,
        AuditPipelineMetrics metrics,
        IOptions<AuditPipelineOptions> options,
        ILogger<AuditLogBatchProcessor> logger)
    {
        _buffer = buffer;
        _primaryDispatcher = primaryDispatcher;
        _fallbackDispatcher = new LoggingAuditLogDispatcher(logger);
        _circuitBreaker = circuitBreaker;
        _metrics = metrics;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Audit log batch processor started. BatchSize={BatchSize}, FlushInterval={FlushInterval}ms",
            _options.BatchSize, _options.FlushIntervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var batch = await _buffer.ReadBatchAsync(
                    _options.BatchSize,
                    TimeSpan.FromMilliseconds(_options.FlushIntervalMs),
                    stoppingToken);

                if (batch.Count == 0)
                    continue;

                _metrics.RecordBatchSize(batch.Count);

                var sw = Stopwatch.StartNew();
                await DispatchWithCircuitBreakerAsync(batch, stoppingToken);
                sw.Stop();

                _metrics.RecordDispatchLatency(sw.Elapsed.TotalMilliseconds);
                _metrics.IncrementDispatchedLogs(batch.Count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown — drain remaining records
                _logger.LogInformation("Audit log batch processor shutting down. Draining buffer...");
                await DrainBufferAsync();
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in audit log batch processor.");
                // Small delay to avoid tight loop on persistent errors
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("Audit log batch processor stopped.");
    }

    private async Task DispatchWithCircuitBreakerAsync(
        IReadOnlyList<AuditLogRecord> batch,
        CancellationToken cancellationToken)
    {
        var dispatcher = _circuitBreaker.IsOpen ? _fallbackDispatcher : _primaryDispatcher;

        try
        {
            await dispatcher.DispatchAsync(batch, cancellationToken);
            _circuitBreaker.RecordSuccess();
        }
        catch (Exception ex)
        {
            _circuitBreaker.RecordFailure();
            _metrics.IncrementDispatchFailures();

            _logger.LogWarning(ex, "Dispatch failed for batch of {Count} records. Circuit breaker state updated.", batch.Count);

            // Try fallback if primary failed
            if (dispatcher != _fallbackDispatcher)
            {
                try
                {
                    await _fallbackDispatcher.DispatchAsync(batch, cancellationToken);
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Fallback dispatcher also failed for batch of {Count} records.", batch.Count);
                }
            }
        }
    }

    private async Task DrainBufferAsync()
    {
        try
        {
            _buffer.Complete();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (true)
            {
                var batch = await _buffer.ReadBatchAsync(
                    _options.BatchSize,
                    TimeSpan.FromMilliseconds(100),
                    cts.Token);

                if (batch.Count == 0)
                    break;

                await _fallbackDispatcher.DispatchAsync(batch, cts.Token);
                _logger.LogDebug("Drained {Count} audit log records during shutdown.", batch.Count);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Drain timed out — some audit log records may have been lost.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during buffer drain.");
        }
    }

    /// <summary>
    /// Internal fallback dispatcher that writes to structured logging.
    /// Used when the circuit breaker is open or during shutdown drain.
    /// </summary>
    private sealed class LoggingAuditLogDispatcher(ILogger logger) : IAuditLogDispatcher
    {
        public Task DispatchAsync(IReadOnlyList<AuditLogRecord> records, CancellationToken cancellationToken = default)
        {
            foreach (var record in records)
            {
                logger.LogWarning(
                    "[AUDIT FALLBACK] {ActionType} {ActionName} | User={UserId} | Tenant={TenantId} | Status={Status} | Duration={DurationMs}ms",
                    record.ActionType, record.ActionName, record.UserId, record.TenantId, record.Status, record.DurationMs);
            }
            return Task.CompletedTask;
        }
    }
}
