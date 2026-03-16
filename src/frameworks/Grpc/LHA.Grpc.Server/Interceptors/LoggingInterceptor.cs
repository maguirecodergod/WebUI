using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace LHA.Grpc.Server.Interceptors;

/// <summary>
/// Structured logging for all gRPC call types.
/// Logs method name, duration, and outcome.
/// </summary>
public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("gRPC call started: {Method}", context.Method);
        try
        {
            var response = await continuation(request, context);
            logger.LogInformation("gRPC call completed: {Method} in {Duration}ms",
                context.Method, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "gRPC call failed: {Method} in {Duration}ms",
                context.Method, sw.ElapsedMilliseconds);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("gRPC server-streaming started: {Method}", context.Method);
        try { await continuation(request, responseStream, context); }
        finally
        {
            logger.LogInformation("gRPC server-streaming ended: {Method} in {Duration}ms",
                context.Method, sw.ElapsedMilliseconds);
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("gRPC client-streaming started: {Method}", context.Method);
        try { return await continuation(requestStream, context); }
        finally
        {
            logger.LogInformation("gRPC client-streaming ended: {Method} in {Duration}ms",
                context.Method, sw.ElapsedMilliseconds);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("gRPC duplex-streaming started: {Method}", context.Method);
        try { await continuation(requestStream, responseStream, context); }
        finally
        {
            logger.LogInformation("gRPC duplex-streaming ended: {Method} in {Duration}ms",
                context.Method, sw.ElapsedMilliseconds);
        }
    }
}
