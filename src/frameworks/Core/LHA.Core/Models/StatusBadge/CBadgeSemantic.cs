namespace LHA;

/// <summary>
/// Defines the semantic meaning of a status, which can be mapped to 
/// specific colors/styles in different UI themes.
/// </summary>
public enum CBadgeSemantic
{
    // System / Generic
    Unknown,
    Processing,
    Pending,
    Completed,
    Failed,
    Cancelled,
    Timeout,
    Retrying,

    // HTTP Status Groups
    Http1xx,
    Http2xx,
    Http3xx,
    Http4xx,
    Http5xx,
    
    // HTTP Methods
    HttpGet,
    HttpPost,
    HttpPut,
    HttpPatch,
    HttpDelete,
    HttpHead,
    HttpOptions,
    HttpTrace,
    HttpConnect,

    // Business Status
    Active,
    Inactive,
    Archived,
    Deleted,
    
    // Request Channel / Type
    RequestHttp,
    RequestGrpc,
    RequestWebhook,
    RequestQueue,
    RequestJob
}
