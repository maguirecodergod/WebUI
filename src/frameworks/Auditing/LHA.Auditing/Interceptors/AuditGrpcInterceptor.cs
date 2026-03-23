using System.Diagnostics;
using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.Core.Users;

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

    public AuditGrpcInterceptor(
        IAuditLogCollector collector,
        ICurrentUser currentUser)
    {
        _collector = collector;
        _currentUser = currentUser;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var record = CreateRecord(context);
        record.RequestBody = SerializePayload(request);

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
        var record = CreateRecord(context);
        record.RequestBody = SerializePayload(request);

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
        var record = CreateRecord(context);
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await continuation(requestStream, context);
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
            _collector.Collect(record);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var record = CreateRecord(context);
        var sw = Stopwatch.StartNew();
        try
        {
            await continuation(requestStream, responseStream, context);
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

    private AuditLogRecord CreateRecord(ServerCallContext context)
    {
        return new AuditLogRecord
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = AuditActionType.GrpcCall,
            ActionName = context.Method,
            RequestPath = context.Method,
            UserId = _currentUser.Id?.ToString(),
            TenantId = _currentUser.TenantId?.ToString(),
            UserName = _currentUser.UserName,
            Roles = _currentUser.Roles.Length > 0
                ? string.Join(',', _currentUser.Roles)
                : null,
            ClientIp = context.Peer
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
}
