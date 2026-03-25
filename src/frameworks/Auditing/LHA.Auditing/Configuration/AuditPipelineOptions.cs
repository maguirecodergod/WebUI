namespace LHA.Auditing.Pipeline;

/// <summary>
/// Configuration options for the high-performance audit logging pipeline.
/// </summary>
public sealed class AuditPipelineOptions
{
    /// <summary>
    /// Master switch. When <c>false</c>, the pipeline is completely disabled
    /// and no records are collected. Default: <c>true</c>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Service/application name stamped on every audit log record.
    /// </summary>
    public string ServiceName { get; set; } = "Unknown";

    /// <summary>
    /// Instance identifier for horizontal scaling scenarios.
    /// Defaults to machine name if not set.
    /// </summary>
    public string? InstanceId { get; set; }

    /// <summary>
    /// Whether to capture the HTTP request body. Default: <c>true</c>.
    /// </summary>
    public bool CaptureRequestBody { get; set; } = true;

    /// <summary>
    /// Whether to capture the HTTP response body. Default: <c>false</c>.
    /// Can be expensive for large responses.
    /// </summary>
    public bool CaptureResponseBody { get; set; }

    /// <summary>
    /// Maximum body size (in bytes) to capture. Bodies exceeding this
    /// are truncated with a "[truncated]" marker. Default: 32 KB.
    /// </summary>
    public int MaxBodySizeBytes { get; set; } = 32 * 1024;

    /// <summary>
    /// Bounded channel capacity. When the buffer is full, the oldest
    /// records are dropped. Default: 100,000.
    /// </summary>
    public int BufferCapacity { get; set; } = 100_000;

    /// <summary>
    /// Number of records per dispatch batch. Default: 500.
    /// </summary>
    public int BatchSize { get; set; } = 500;

    /// <summary>
    /// Maximum time (ms) to wait before flushing a partial batch.
    /// Default: 2000 ms.
    /// </summary>
    public int FlushIntervalMs { get; set; } = 2_000;

    /// <summary>
    /// Sampling rate for audit logs (0.0 to 1.0).
    /// 1.0 = capture all, 0.1 = capture 10% (probabilistic).
    /// Default: 1.0.
    /// </summary>
    public double SamplingRate { get; set; } = 1.0;

    /// <summary>
    /// When <c>true</c>, always capture logs for failed requests
    /// regardless of sampling rate. Default: <c>true</c>.
    /// </summary>
    public bool AlwaysLogOnException { get; set; } = true;

    /// <summary>
    /// Circuit breaker: number of consecutive dispatch failures
    /// before switching to fallback dispatcher. Default: 5.
    /// </summary>
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker: time (ms) to wait before attempting to
    /// recover from fallback mode. Default: 30,000 ms.
    /// </summary>
    public int CircuitBreakerRecoveryMs { get; set; } = 30_000;

    /// <summary>
    /// Sensitive field names to auto-mask (case-insensitive).
    /// </summary>
    public HashSet<string> SensitiveFieldNames { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "passwd",
        "secret",
        "token",
        "accessToken",
        "refreshToken",
        "apiKey",
        "creditCard",
        "cardNumber",
        "cvv",
        "ssn",
        "authorization"
    };

    /// <summary>
    /// URL paths to exclude from audit logging (exact match, case-insensitive).
    /// </summary>
    public HashSet<string> ExcludedPaths { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/healthz",
        "/ready",
        "/metrics",
        "/favicon.ico",
        "/swagger",
        "/swagger/index.html",
        "/swagger/v1/swagger.json",
        "/swagger/v1/swagger.yaml",
        "/swagger/v1/swagger.xml",
        "/scalars",
        "/scalars/index.html",
        "/scalars/v1/schema.json",
        "/scalars/v1/schema.yaml",
        "/scalars/v1/schema.xml",
        "/openapi",
        "/openapi/v1.json"
    };

    /// <summary>
    /// When <c>true</c>, suppresses all exceptions thrown during audit pipeline
    /// processing. Default: <c>true</c>.
    /// </summary>
    public bool HideErrors { get; set; } = true;

    /// <summary>
    /// Whether to compress large request/response bodies using Brotli.
    /// Applied only when body size exceeds <see cref="CompressionThresholdBytes"/>.
    /// Default: <c>false</c>.
    /// </summary>
    public bool EnableCompression { get; set; }

    /// <summary>
    /// Body size (in bytes) above which compression is applied.
    /// Default: 4 KB.
    /// </summary>
    public int CompressionThresholdBytes { get; set; } = 4 * 1024;
}
