using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Default <see cref="IAuditLogCollector"/> that applies enrichment, masking,
/// sampling, and then writes to the <see cref="IAuditLogBuffer"/>.
/// <para>
/// Designed to be called from interceptors on the hot path.
/// All operations are synchronous and non-blocking.
/// </para>
/// </summary>
internal sealed class ChannelAuditLogCollector : IAuditLogCollector
{
    private readonly IAuditLogBuffer _buffer;
    private readonly IAuditDataMasker _masker;
    private readonly IEnumerable<IAuditLogEnricher> _enrichers;
    private readonly AuditPipelineOptions _options;
    private readonly AuditPipelineMetrics _metrics;
    private readonly ILogger<ChannelAuditLogCollector> _logger;

    // Thread-safe random for sampling
    [ThreadStatic]
    private static Random? t_random;
    private static Random ThreadRandom => t_random ??= new Random();

    public ChannelAuditLogCollector(
        IAuditLogBuffer buffer,
        IAuditDataMasker masker,
        IEnumerable<IAuditLogEnricher> enrichers,
        IOptions<AuditPipelineOptions> options,
        AuditPipelineMetrics metrics,
        ILogger<ChannelAuditLogCollector> logger)
    {
        _buffer = buffer;
        _masker = masker;
        _enrichers = enrichers;
        _options = options.Value;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool Collect(AuditLogRecord record)
    {
        if (!_options.Enabled)
            return false;

        _metrics.IncrementCollectedLogs();

        // Sampling — always log failures if configured
        if (_options.SamplingRate < 1.0)
        {
            var isFailure = record.Status == AuditLogStatus.Failure;
            if (!(_options.AlwaysLogOnException && isFailure))
            {
                if (ThreadRandom.NextDouble() > _options.SamplingRate)
                {
                    _metrics.IncrementDroppedLogs();
                    return false;
                }
            }
        }

        try
        {
            // Apply enrichers
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(record);
            }

            // Apply masking
            record.RequestBody = _masker.MaskJson(record.RequestBody);
            record.ResponseBody = _masker.MaskJson(record.ResponseBody);

            // Set service info defaults
            record.ServiceName ??= _options.ServiceName;
            record.InstanceId ??= _options.InstanceId ?? Environment.MachineName;

            // Write to buffer (non-blocking)
            return _buffer.TryWrite(record);
        }
        catch (Exception ex)
        {
            if (_options.HideErrors)
            {
                _logger.LogWarning(ex, "Failed to collect audit log record for {ActionName}.", record.ActionName);
                return false;
            }
            throw;
        }
    }
}
