namespace LHA.Auditing.Pipeline;

/// <summary>
/// Classifies the type of operation captured in an audit log record.
/// </summary>
public enum AuditActionType : byte
{
    /// <summary>
    /// 0 - HttpRequest: HTTP request (Minimal API / Controller endpoint).
    /// </summary>
    HttpRequest = 0,

    /// <summary>
    /// 1 - GrpcCall: gRPC unary, streaming, or duplex call.
    /// </summary>
    GrpcCall = 1,

    /// <summary>
    /// 2 - BackgroundJob: Background job execution.
    /// </summary>
    BackgroundJob = 2,

    /// <summary>
    /// 3 - EventHandler: EventBus handler invocation.
    /// </summary>
    EventHandler = 3
}
