using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// <see cref="IAuditLogBuffer"/> backed by a bounded <see cref="Channel{T}"/>.
/// <para>
/// Lock-free, high-throughput, backpressure-aware.
/// When the buffer is full, the oldest records are dropped (DropOldest policy)
/// and the <see cref="AuditPipelineMetrics.DroppedLogs"/> counter is incremented.
/// </para>
/// </summary>
internal sealed class ChannelAuditLogBuffer : IAuditLogBuffer
{
    private readonly Channel<AuditLogRecord> _channel;
    private readonly AuditPipelineMetrics _metrics;

    public ChannelAuditLogBuffer(
        IOptions<AuditPipelineOptions> options,
        AuditPipelineMetrics metrics)
    {
        _metrics = metrics;

        var opts = options.Value;
        _channel = Channel.CreateBounded<AuditLogRecord>(new BoundedChannelOptions(opts.BufferCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = false,
            SingleReader = true, // Batch processor is the only reader
            AllowSynchronousContinuations = false
        });
    }

    /// <inheritdoc />
    public bool TryWrite(AuditLogRecord record)
    {
        var written = _channel.Writer.TryWrite(record);
        if (!written)
        {
            _metrics.IncrementDroppedLogs();
        }
        else
        {
            _metrics.IncrementBufferedLogs();
        }
        return written;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLogRecord>> ReadBatchAsync(
        int maxBatchSize,
        TimeSpan maxWait,
        CancellationToken cancellationToken)
    {
        var batch = new List<AuditLogRecord>(maxBatchSize);

        // Wait for at least one item
        if (!await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            return batch; // Channel completed
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(maxWait);

        try
        {
            while (batch.Count < maxBatchSize)
            {
                if (_channel.Reader.TryRead(out var record))
                {
                    batch.Add(record);
                }
                else if (batch.Count > 0)
                {
                    // Have some items but channel is temporarily empty — flush partial
                    break;
                }
                else
                {
                    // Wait for more items (will be cancelled by maxWait)
                    if (!await _channel.Reader.WaitToReadAsync(cts.Token))
                        break;
                }
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // maxWait expired — flush what we have (normal operation)
        }

        return batch;
    }

    /// <inheritdoc />
    public void Complete()
    {
        _channel.Writer.TryComplete();
    }
}
