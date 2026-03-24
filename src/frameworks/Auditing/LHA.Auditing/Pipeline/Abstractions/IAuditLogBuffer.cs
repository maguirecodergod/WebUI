namespace LHA.Auditing.Pipeline;

/// <summary>
/// Bounded buffer for audit log records.
/// Backed by <see cref="System.Threading.Channels.Channel{T}"/>
/// for lock-free, high-throughput producer/consumer semantics.
/// </summary>
public interface IAuditLogBuffer
{
    /// <summary>
    /// Writes a record to the buffer. Non-blocking.
    /// </summary>
    /// <returns><c>true</c> if written; <c>false</c> if buffer is full and record was dropped.</returns>
    bool TryWrite(AuditLogRecord record);

    /// <summary>
    /// Reads a batch of records from the buffer.
    /// Blocks until at least one record is available or cancellation is requested.
    /// </summary>
    /// <param name="maxBatchSize">Maximum number of records to read in one batch.</param>
    /// <param name="maxWait">Maximum time to wait for a full batch before flushing partial.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A batch of records (may be smaller than <paramref name="maxBatchSize"/>).</returns>
    Task<IReadOnlyList<AuditLogRecord>> ReadBatchAsync(
        int maxBatchSize,
        TimeSpan maxWait,
        CancellationToken cancellationToken);

    /// <summary>
    /// Signals that no more records will be written.
    /// </summary>
    void Complete();
}
