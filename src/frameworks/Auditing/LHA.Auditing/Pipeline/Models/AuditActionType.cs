namespace LHA.Auditing.Pipeline;

/// <summary>
/// Classifies the type of operation captured in an audit log record.
/// </summary>
public enum AuditActionType : byte
{
    /// <summary>HTTP request (Minimal API / Controller endpoint).</summary>
    HttpRequest = 0,

    /// <summary>gRPC unary, streaming, or duplex call.</summary>
    GrpcCall = 1,

    /// <summary>Background job execution.</summary>
    BackgroundJob = 2,

    /// <summary>EventBus handler invocation.</summary>
    EventHandler = 3
}
