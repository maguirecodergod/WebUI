namespace LHA.MessageBroker;

/// <summary>
/// Handler interface for processing messages from any broker.
/// Implement this interface per message type and register in DI.
/// The broker-specific consumer infrastructure will dispatch messages to the appropriate handler.
/// </summary>
/// <typeparam name="T">The message payload type.</typeparam>
public interface IMessageHandler<T> where T : class
{
    /// <summary>
    /// Handles a received message.
    /// </summary>
    /// <param name="context">The message context containing payload, headers, and metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the message was handled successfully; false to trigger retry.</returns>
    Task<bool> HandleAsync(MessageContext<T> context, CancellationToken cancellationToken = default);
}
