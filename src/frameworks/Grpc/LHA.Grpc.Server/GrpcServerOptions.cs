using System.IO.Compression;
using System.Threading.RateLimiting;

namespace LHA.Grpc.Server;

public sealed class GrpcServerOptions
{
    /// <summary>Include exception details in error responses (disable in production).</summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>Maximum receive message size in bytes. null = default (4 MB).</summary>
    public int? MaxReceiveMessageSize { get; set; }

    /// <summary>Maximum send message size in bytes. null = default (4 MB).</summary>
    public int? MaxSendMessageSize { get; set; }

    /// <summary>Response compression algorithm, e.g. "gzip".</summary>
    public string? ResponseCompressionAlgorithm { get; set; }

    /// <summary>Compression level when algorithm is set.</summary>
    public CompressionLevel ResponseCompressionLevel { get; set; } = CompressionLevel.Fastest;

    /// <summary>Whether to register gRPC server reflection for service discovery.</summary>
    public bool EnableReflection { get; set; } = true;

    /// <summary>Rate limiter applied to all gRPC calls. null = no rate limiting.</summary>
    public RateLimiter? RateLimiter { get; set; }
}
