using LHA;

namespace LHA.Auditing;

/// <summary>
/// Classifies the transport/request channel that produced an audit record.
/// </summary>
public enum CRequestType : byte
{
    /// <summary>Unknown or not classified.</summary>
    [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi bi-question-circle")]
    Unknown = 0,

    /// <summary>Regular HTTP request/response.</summary>
    [StatusBadge(CBadgeSemantic.RequestHttp, Icon = "bi bi-globe")]
    Http = 1,

    /// <summary>gRPC request over HTTP/2.</summary>
    [StatusBadge(CBadgeSemantic.RequestGrpc, Icon = "bi bi-lightning-charge")]
    Grpc = 2,

    /// <summary>Webhook request from external systems.</summary>
    [StatusBadge(CBadgeSemantic.RequestWebhook, Icon = "bi bi-link-45deg")]
    Webhook = 3,

    /// <summary>Message queue consumer processing (Kafka/RabbitMQ/etc.).</summary>
    [StatusBadge(CBadgeSemantic.RequestQueue, Icon = "bi bi-stack")]
    MessageQueue = 4,

    /// <summary>Background job execution.</summary>
    [StatusBadge(CBadgeSemantic.RequestJob, Icon = "bi bi-clock-history")]
    BackgroundJob = 5
}
