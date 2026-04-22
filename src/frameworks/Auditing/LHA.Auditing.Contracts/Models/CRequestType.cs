namespace LHA.Auditing;

/// <summary>
/// Classifies the transport/request channel that produced an audit record.
/// </summary>
public enum CRequestType : byte
{
    /// <summary>Unknown or not classified.</summary>
    Unknown = 0,

    /// <summary>Regular HTTP request/response.</summary>
    Http = 1,

    /// <summary>gRPC request over HTTP/2.</summary>
    Grpc = 2,

    /// <summary>Webhook request from external systems.</summary>
    Webhook = 3,

    /// <summary>Message queue consumer processing (Kafka/RabbitMQ/etc.).</summary>
    MessageQueue = 4,

    /// <summary>Background job execution.</summary>
    BackgroundJob = 5
}
