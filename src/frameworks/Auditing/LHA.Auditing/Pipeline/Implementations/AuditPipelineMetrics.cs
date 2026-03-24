using System.Diagnostics.Metrics;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// OpenTelemetry metrics for the audit logging pipeline.
/// Exposes counters and histograms for observability dashboards.
/// </summary>
public sealed class AuditPipelineMetrics
{
    public const string MeterName = "LHA.Auditing.Pipeline";

    private readonly Counter<long> _logsCollected;
    private readonly Counter<long> _logsDropped;
    private readonly Counter<long> _logsBuffered;
    private readonly Counter<long> _logsDispatched;
    private readonly Counter<long> _dispatchFailures;
    private readonly Histogram<double> _batchSize;
    private readonly Histogram<double> _dispatchLatency;

    public AuditPipelineMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _logsCollected = meter.CreateCounter<long>(
            "audit.logs.collected",
            description: "Total audit log records collected");

        _logsDropped = meter.CreateCounter<long>(
            "audit.logs.dropped",
            description: "Audit log records dropped due to buffer overflow or sampling");

        _logsBuffered = meter.CreateCounter<long>(
            "audit.logs.buffered",
            description: "Audit log records written to the buffer");

        _logsDispatched = meter.CreateCounter<long>(
            "audit.logs.dispatched",
            description: "Audit log records successfully dispatched");

        _dispatchFailures = meter.CreateCounter<long>(
            "audit.dispatch.failures",
            description: "Number of dispatch batch failures");

        _batchSize = meter.CreateHistogram<double>(
            "audit.batch.size",
            description: "Size of dispatched batches");

        _dispatchLatency = meter.CreateHistogram<double>(
            "audit.dispatch.latency_ms",
            unit: "ms",
            description: "Dispatch latency per batch in milliseconds");
    }

    public void IncrementCollectedLogs() => _logsCollected.Add(1);
    public void IncrementDroppedLogs() => _logsDropped.Add(1);
    public void IncrementBufferedLogs() => _logsBuffered.Add(1);
    public void IncrementDispatchedLogs(int count) => _logsDispatched.Add(count);
    public void IncrementDispatchFailures() => _dispatchFailures.Add(1);
    public void RecordBatchSize(int size) => _batchSize.Record(size);
    public void RecordDispatchLatency(double latencyMs) => _dispatchLatency.Record(latencyMs);
}
