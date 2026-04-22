using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// gRPC server interceptor that automatically captures audit log records
/// for all unary, server-streaming, client-streaming, and duplex calls.
/// <para>
/// Captures gRPC method name, metadata (headers), request payload,
/// user context, and execution metrics.
/// </para>
/// </summary>
public sealed class AuditGrpcInterceptor : Interceptor
{
    private readonly IAuditLogCollector _collector;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AuditGrpcInterceptor> _logger;

    public AuditGrpcInterceptor(
        IAuditLogCollector collector,
        ICurrentUser currentUser,
        ILogger<AuditGrpcInterceptor> logger)
    {
        _collector = collector;
        _currentUser = currentUser;
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("AUDIT_GRPC_INTERCEPTOR_HIT Unary {Method}", context.Method);
        var record = CreateRecord(context);
        record.RequestBody = SerializePayload(request);
        TrySyncRequestBodyToDataAudit(context, record.RequestBody);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await continuation(request, context);
            sw.Stop();

            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Success;
            record.StatusCode = (int)context.Status.StatusCode;

            return response;
        }
        catch (RpcException ex)
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Failure;
            record.StatusCode = (int)ex.StatusCode;
            record.Exception = AuditExceptionSerializer.Serialize(ex);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Failure;
            record.Exception = AuditExceptionSerializer.Serialize(ex);
            throw;
        }
        finally
        {
            _collector.Collect(record);
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("AUDIT_GRPC_INTERCEPTOR_HIT ServerStreaming {Method}", context.Method);
        var record = CreateRecord(context);
        record.RequestBody = SerializePayload(request);
        TrySyncRequestBodyToDataAudit(context, record.RequestBody);

        var sw = Stopwatch.StartNew();
        try
        {
            await continuation(request, responseStream, context);
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Success;
        }
        catch (Exception ex)
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Failure;
            record.Exception = AuditExceptionSerializer.Serialize(ex);
            throw;
        }
        finally
        {
            _collector.Collect(record);
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("AUDIT_GRPC_INTERCEPTOR_HIT ClientStreaming {Method}", context.Method);
        var record = CreateRecord(context);
        var recordingStream = new RecordingAsyncStreamReader<TRequest>(requestStream);
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await continuation(recordingStream, context);
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Success;
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Failure;
            record.Exception = AuditExceptionSerializer.Serialize(ex);
            throw;
        }
        finally
        {
            record.RequestBody = recordingStream.GetSerializedPayload();
            TrySyncRequestBodyToDataAudit(context, record.RequestBody);
            _collector.Collect(record);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("AUDIT_GRPC_INTERCEPTOR_HIT Duplex {Method}", context.Method);
        var record = CreateRecord(context);
        var recordingStream = new RecordingAsyncStreamReader<TRequest>(requestStream);
        var sw = Stopwatch.StartNew();
        try
        {
            await continuation(recordingStream, responseStream, context);
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Success;
        }
        catch (Exception ex)
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;
            record.Status = AuditLogStatus.Failure;
            record.Exception = AuditExceptionSerializer.Serialize(ex);
            throw;
        }
        finally
        {
            record.RequestBody = recordingStream.GetSerializedPayload();
            TrySyncRequestBodyToDataAudit(context, record.RequestBody);
            _collector.Collect(record);
        }
    }

    private AuditLogRecord CreateRecord(ServerCallContext context)
    {
        return new AuditLogRecord
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = AuditActionType.GrpcCall,
            RequestType = CRequestType.Grpc,
            ActionName = context.Method,
            RequestPath = context.Method,
            UserId = _currentUser.Id?.ToString(),
            TenantId = _currentUser.TenantId?.ToString(),
            UserName = _currentUser.UserName,
            Roles = _currentUser.Roles.Length > 0
                ? string.Join(',', _currentUser.Roles)
                : null,
            ClientIp = context.Peer,
            Tags = JsonSerializer.Serialize(new Dictionary<string, string?>
            {
                ["requestType"] = CRequestType.Grpc.ToString(),
                ["grpcMethod"] = context.Method,
                ["grpcHost"] = context.Host,
                ["grpcPeer"] = context.Peer
            })
        };
    }

    private static string? SerializePayload<T>(T payload)
    {
        try
        {
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            return null;
        }
    }

    private static void TrySyncRequestBodyToDataAudit(ServerCallContext context, string? requestBody)
    {
        if (string.IsNullOrWhiteSpace(requestBody))
            return;

        var httpContext = context.GetHttpContext();
        var auditingManager = httpContext?.RequestServices.GetService<IAuditingManager>();
        var log = auditingManager?.Current?.Log;
        if (log is null)
            return;

        log.ExtraProperties["RequestBody"] = requestBody;
    }

    private sealed class RecordingAsyncStreamReader<T>(
        IAsyncStreamReader<T> inner,
        int maxMessages = 20,
        int maxChars = 64 * 1024) : IAsyncStreamReader<T>
    {
        private readonly List<string> _messages = [];
        private int _currentChars;

        public T Current => inner.Current;

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            var hasNext = await inner.MoveNext(cancellationToken);
            if (!hasNext)
                return false;

            if (_messages.Count < maxMessages && _currentChars < maxChars)
            {
                var json = SerializePayload(inner.Current);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _messages.Add(json);
                    _currentChars += json.Length;
                }
            }

            return true;
        }

        public string? GetSerializedPayload()
        {
            if (_messages.Count == 0)
                return null;

            var sb = new StringBuilder();
            sb.Append('[');
            sb.Append(string.Join(",", _messages));
            if (_messages.Count >= maxMessages || _currentChars >= maxChars)
            {
                sb.Append(",{\"truncated\":true}");
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
