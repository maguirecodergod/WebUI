using System.Threading.RateLimiting;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LHA.Grpc.Server.Interceptors;

/// <summary>
/// Applies rate limiting to incoming gRPC calls.
/// Returns RESOURCE_EXHAUSTED when the limit is exceeded.
/// Requires a <see cref="RateLimiter"/> registered in DI.
/// </summary>
public sealed class RateLimitInterceptor(RateLimiter rateLimiter) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        await AcquireOrThrowAsync(context.CancellationToken);
        return await continuation(request, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await AcquireOrThrowAsync(context.CancellationToken);
        await continuation(request, responseStream, context);
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await AcquireOrThrowAsync(context.CancellationToken);
        return await continuation(requestStream, context);
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await AcquireOrThrowAsync(context.CancellationToken);
        await continuation(requestStream, responseStream, context);
    }

    private async Task AcquireOrThrowAsync(CancellationToken ct)
    {
        using var lease = await rateLimiter.AcquireAsync(1, ct);
        if (!lease.IsAcquired)
            throw new RpcException(new Status(StatusCode.ResourceExhausted,
                "Rate limit exceeded. Retry after a short delay."));
    }
}
