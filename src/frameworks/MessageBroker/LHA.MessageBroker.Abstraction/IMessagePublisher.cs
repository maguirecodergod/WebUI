namespace LHA.MessageBroker;

/// <summary>
/// Unified abstraction for publishing messages to any broker (Kafka, RabbitMQ, etc.).
/// Application services should depend on this interface, not on broker-specific implementations.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a single message to the specified destination.
    /// </summary>
    /// <typeparam name="T">Payload type.</typeparam>
    /// <param name="destination">
    /// Logical destination name. Interpretation depends on broker:
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> topic name</item>
    ///   <item><b>RabbitMQ:</b> exchange name</item>
    /// </list>
    /// </param>
    /// <param name="envelope">The message envelope containing payload and metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A broker-agnostic result with optional broker-specific metadata.</returns>
    Task<PublishResult> PublishAsync<T>(
        string destination,
        MessageEnvelope<T> envelope,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes a batch of messages to the specified destination.
    /// Implementations should optimize for throughput (batching, pipelining).
    /// </summary>
    Task<IReadOnlyList<PublishResult>> PublishBatchAsync<T>(
        string destination,
        IEnumerable<MessageEnvelope<T>> envelopes,
        CancellationToken cancellationToken = default) where T : class;
}
