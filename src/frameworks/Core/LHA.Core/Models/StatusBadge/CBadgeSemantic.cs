namespace LHA;

/// <summary>
/// Defines the semantic meaning of a status, which can be mapped to 
/// specific colors/styles in different UI themes.
/// </summary>
public enum CBadgeSemantic
{
    // System / Generic
    /// <summary>
    /// 0 - Unknown
    /// </summary>
    Unknown,
    /// <summary>
    /// 1 - Processing
    /// </summary>
    Processing,
    /// <summary>
    /// 2 - Pending
    /// </summary>
    Pending,
    /// <summary>
    /// 3 - Completed
    /// </summary>
    Completed,
    /// <summary>
    /// 4 - Failed
    /// </summary>
    Failed,
    /// <summary>
    /// 5 - Cancelled
    /// </summary>
    Cancelled,
    /// <summary>
    /// 6 - Timeout
    /// </summary>
    Timeout,
    /// <summary>
    /// 7 - Retrying
    /// </summary>
    Retrying,

    // HTTP Status Groups
    /// <summary>
    /// 8 - Http1xx
    /// </summary>
    Http1xx,
    /// <summary>
    /// 9 - Http2xx
    /// </summary>
    Http2xx,
    /// <summary>
    /// 10 - Http3xx
    /// </summary>
    Http3xx,
    /// <summary>
    /// 11 - Http4xx
    /// </summary>
    Http4xx,
    /// <summary>
    /// 12 - Http5xx
    /// </summary>
    Http5xx,
    
    // HTTP Methods
    /// <summary>
    /// 13 - HttpGet
    /// </summary>
    HttpGet,
    /// <summary>
    /// 14 - HttpPost
    /// </summary>
    HttpPost,
    /// <summary>
    /// 15 - HttpPut
    /// </summary>
    HttpPut,
    /// <summary>
    /// 16 - HttpPatch
    /// </summary>
    HttpPatch,
    /// <summary>
    /// 17 - HttpDelete
    /// </summary>
    HttpDelete,
    /// <summary>
    /// 18 - HttpHead
    /// </summary>
    HttpHead,
    /// <summary>
    /// 19 - HttpOptions
    /// </summary>
    HttpOptions,
    /// <summary>
    /// 20 - HttpTrace
    /// </summary>
    HttpTrace,
    /// <summary>
    /// 21 - HttpConnect
    /// </summary>
    HttpConnect,

    // Business Status
    /// <summary>
    /// 22 - Active
    /// </summary>
    Active,
    /// <summary>
    /// 23 - Inactive
    /// </summary>
    Inactive,
    /// <summary>
    /// 24 - Archived
    /// </summary>
    Archived,
    /// <summary>
    /// 25 - Deleted
    /// </summary>
    Deleted,
    
    // Request Channel / Type
    /// <summary>
    /// 26 - RequestHttp
    /// </summary>
    RequestHttp,
    /// <summary>
    /// 27 - RequestGrpc
    /// </summary>
    RequestGrpc,
    /// <summary>
    /// 28 - RequestWebhook
    /// </summary>
    RequestWebhook,
    /// <summary>
    /// 29 - RequestQueue
    /// </summary>
    RequestQueue,
    /// <summary>
    /// 30 - RequestJob
    /// </summary>
    RequestJob
}
