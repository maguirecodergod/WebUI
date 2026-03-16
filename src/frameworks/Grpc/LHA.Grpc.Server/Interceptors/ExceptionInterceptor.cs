using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace LHA.Grpc.Server.Interceptors;

/// <summary>
/// Catches unhandled exceptions and maps them to appropriate gRPC status codes.
/// Position: innermost server interceptor (closest to the service handler).
/// </summary>
public sealed class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try { return await continuation(request, context); }
        catch (Exception ex) { throw Handle(ex, context); }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { await continuation(request, responseStream, context); }
        catch (Exception ex) { throw Handle(ex, context); }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { return await continuation(requestStream, context); }
        catch (Exception ex) { throw Handle(ex, context); }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { await continuation(requestStream, responseStream, context); }
        catch (Exception ex) { throw Handle(ex, context); }
    }

    private RpcException Handle(Exception ex, ServerCallContext context)
    {
        if (ex is RpcException rpc) return rpc;

        var (code, message) = ex switch
        {
            ArgumentException          => (StatusCode.InvalidArgument,   ex.Message),
            KeyNotFoundException       => (StatusCode.NotFound,          ex.Message),
            UnauthorizedAccessException=> (StatusCode.PermissionDenied,  ex.Message),
            InvalidOperationException  => (StatusCode.FailedPrecondition,ex.Message),
            NotImplementedException    => (StatusCode.Unimplemented,     ex.Message),
            OperationCanceledException => (StatusCode.Cancelled,         "Operation was cancelled."),
            _                          => (StatusCode.Internal,          "An internal error occurred."),
        };

        logger.LogError(ex, "gRPC {Method} failed with {StatusCode}", context.Method, code);

        var trailers = new Metadata();
        if (Activity.Current?.Id is { } traceId)
            trailers.Add("x-trace-id", traceId);

        return new RpcException(new Status(code, message), trailers);
    }
}
