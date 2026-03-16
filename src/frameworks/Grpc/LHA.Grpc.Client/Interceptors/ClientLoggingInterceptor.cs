using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace LHA.Grpc.Client.Interceptors;

/// <summary>
/// Client-side interceptor that logs outgoing gRPC calls with duration tracking.
/// </summary>
public sealed class ClientLoggingInterceptor(ILogger<ClientLoggingInterceptor> logger) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var method = context.Method.FullName;
        logger.LogDebug("gRPC client call: {Method}", method);
        var sw = Stopwatch.StartNew();

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync, method, sw),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        logger.LogDebug("gRPC client server-streaming: {Method}", context.Method.FullName);
        return continuation(request, context);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        logger.LogDebug("gRPC client client-streaming: {Method}", context.Method.FullName);
        return continuation(context);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        logger.LogDebug("gRPC client duplex-streaming: {Method}", context.Method.FullName);
        return continuation(context);
    }

    private async Task<TResponse> HandleResponse<TResponse>(
        Task<TResponse> responseTask, string method, Stopwatch sw)
    {
        try
        {
            var response = await responseTask;
            logger.LogDebug("gRPC client completed: {Method} in {Duration}ms",
                method, sw.ElapsedMilliseconds);
            return response;
        }
        catch (RpcException ex)
        {
            logger.LogWarning("gRPC client failed: {Method} in {Duration}ms Status={Status}",
                method, sw.ElapsedMilliseconds, ex.StatusCode);
            throw;
        }
    }
}
