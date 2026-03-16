using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LHA.Grpc.Client.Interceptors;

/// <summary>
/// Applies a default deadline to outgoing gRPC calls that don't already specify one.
/// Prevents runaway calls in production by enforcing explicit timeouts.
/// </summary>
public sealed class DeadlinePropagationInterceptor(TimeSpan? defaultDeadline) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(request, EnsureDeadline(context));
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(request, EnsureDeadline(context));
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(EnsureDeadline(context));
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(EnsureDeadline(context));
    }

    private ClientInterceptorContext<TRequest, TResponse> EnsureDeadline<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context)
        where TRequest : class
        where TResponse : class
    {
        if (context.Options.Deadline is not null || defaultDeadline is null)
            return context;

        var options = context.Options.WithDeadline(DateTime.UtcNow + defaultDeadline.Value);
        return new ClientInterceptorContext<TRequest, TResponse>(
            context.Method, context.Host, options);
    }
}
