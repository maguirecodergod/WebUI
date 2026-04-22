using Microsoft.AspNetCore.Http;

namespace LHA.Auditing.Interceptors;

internal static class AuditRequestClassifier
{
    public static CRequestType DetectHttpRequestType(HttpRequest request)
    {
        if (IsGrpcRequest(request))
            return CRequestType.Grpc;

        if (IsWebhookRequest(request))
            return CRequestType.Webhook;

        return CRequestType.Http;
    }

    private static bool IsGrpcRequest(HttpRequest request)
    {
        if (request.Protocol.StartsWith("HTTP/2", StringComparison.OrdinalIgnoreCase)
            && request.ContentType?.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return request.Path.Value?.Contains('.', StringComparison.Ordinal) == true
               && request.Headers.ContainsKey("Grpc-Timeout");
    }

    private static bool IsWebhookRequest(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;
        if (path.Contains("webhook", StringComparison.OrdinalIgnoreCase))
            return true;

        var headers = request.Headers;
        return headers.ContainsKey("X-Webhook-Event")
               || headers.ContainsKey("X-GitHub-Event")
               || headers.ContainsKey("X-Gitlab-Event")
               || headers.ContainsKey("X-Hub-Signature")
               || headers.ContainsKey("X-Hub-Signature-256")
               || headers.ContainsKey("X-Signature")
               || headers.ContainsKey("Stripe-Signature");
    }
}
