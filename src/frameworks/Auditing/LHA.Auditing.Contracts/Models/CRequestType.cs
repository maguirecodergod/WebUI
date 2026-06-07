using LHA;

namespace LHA.Auditing;

/// <summary>
/// Classifies the transport/request channel that produced an audit record.
/// </summary>
public enum CRequestType : byte
{
    /// <summary>
    /// 0 - Unknown: The request channel could not be determined.
    /// </summary>
    [StatusBadge(CBadgeSemantic.Unknown, Icon = "bi bi-question-circle")]
    Unknown = 0,

    /// <summary>
    /// 1 - Http: Standard HTTP/HTTPS request.
    /// </summary>
    [StatusBadge(CBadgeSemantic.RequestHttp, Icon = "bi bi-globe")]
    Http = 1,

    /// <summary>
    /// 2 - Grpc: gRPC remote procedure call.
    /// </summary>
    [StatusBadge(CBadgeSemantic.RequestGrpc, Icon = "bi bi-lightning-charge")]
    Grpc = 2,

    /// <summary>
    /// 3 - Webhook: Incoming webhook callback from an external system.
    /// </summary>
    [StatusBadge(CBadgeSemantic.RequestWebhook, Icon = "bi bi-link-45deg")]
    Webhook = 3,

    /// <summary>
    /// 4 - MessageQueue: Message consumed from a message broker (e.g., Kafka).
    /// </summary>
    [StatusBadge(CBadgeSemantic.RequestQueue, Icon = "bi bi-stack")]
    MessageQueue = 4,

    /// <summary>
    /// 5 - BackgroundJob: Background job executed by the job scheduler (e.g., Hangfire).
    /// </summary>
    [StatusBadge(CBadgeSemantic.RequestJob, Icon = "bi bi-clock-history")]
    BackgroundJob = 5
}
