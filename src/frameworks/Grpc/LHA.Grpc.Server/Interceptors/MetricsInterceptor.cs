using System.Diagnostics;
using System.Diagnostics.Metrics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LHA.Grpc.Server.Interceptors;

/// <summary>
/// Records gRPC call metrics using System.Diagnostics.Metrics (OpenTelemetry-compatible).
/// Exposes: grpc.server.calls_total, grpc.server.call_duration_ms, grpc.server.errors_total.
/// Position: outermost server interceptor to capture all outcomes.
/// </summary>
public sealed class MetricsInterceptor : Interceptor
{
    private static readonly Meter s_meter = new("LHA.Grpc.Server", "1.0.0");

    private static readonly Counter<long> s_calls =
        s_meter.CreateCounter<long>("grpc.server.calls_total", description: "Total gRPC calls");

    private static readonly Histogram<double> s_duration =
        s_meter.CreateHistogram<double>("grpc.server.call_duration_ms",
            unit: "ms", description: "gRPC call duration");

    private static readonly Counter<long> s_errors =
        s_meter.CreateCounter<long>("grpc.server.errors_total", description: "Total gRPC errors");

    // ── Unary ────────────────────────────────────────────────────

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
        => RecordAsync(context.Method, "unary", () => continuation(request, context));

    // ── Server streaming ─────────────────────────────────────────

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        => RecordAsync(context.Method, "server_streaming",
            () => continuation(request, responseStream, context));

    // ── Client streaming ─────────────────────────────────────────

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
        => RecordAsync(context.Method, "client_streaming",
            () => continuation(requestStream, context));

    // ── Duplex streaming ─────────────────────────────────────────

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        => RecordAsync(context.Method, "duplex_streaming",
            () => continuation(requestStream, responseStream, context));

    // ── Helpers ──────────────────────────────────────────────────

    private static async Task<T> RecordAsync<T>(string method, string type, Func<Task<T>> func)
    {
        s_calls.Add(1, Tag("grpc.method", method), Tag("grpc.type", type));
        var sw = Stopwatch.StartNew();
        try { return await func(); }
        catch { s_errors.Add(1, Tag("grpc.method", method)); throw; }
        finally { s_duration.Record(sw.Elapsed.TotalMilliseconds, Tag("grpc.method", method)); }
    }

    private static async Task RecordAsync(string method, string type, Func<Task> func)
    {
        s_calls.Add(1, Tag("grpc.method", method), Tag("grpc.type", type));
        var sw = Stopwatch.StartNew();
        try { await func(); }
        catch { s_errors.Add(1, Tag("grpc.method", method)); throw; }
        finally { s_duration.Record(sw.Elapsed.TotalMilliseconds, Tag("grpc.method", method)); }
    }

    private static KeyValuePair<string, object?> Tag(string key, string value) => new(key, value);
}
