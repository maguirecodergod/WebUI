using System.Diagnostics;
using System.Text.Json;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.EventBus;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// Decorator for <see cref="IEventHandler{TEvent}"/> that automatically
/// creates audit log records for every event handler execution.
/// <para>
/// Applied via open-generic decoration in DI.
/// Captures event type, serialized payload, context metadata, and execution metrics.
/// </para>
/// </summary>
public sealed class AuditEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
    where TEvent : class
{
    private readonly IEventHandler<TEvent> _inner;
    private readonly IAuditLogCollector _collector;

    public AuditEventHandlerDecorator(
        IEventHandler<TEvent> inner,
        IAuditLogCollector collector)
    {
        _inner = inner;
        _collector = collector;
    }

    /// <inheritdoc />
    public async Task HandleAsync(TEvent @event, EventContext context, CancellationToken cancellationToken = default)
    {
        var record = new AuditLogRecord
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = AuditActionType.EventHandler,
            ActionName = $"EventHandler:{_inner.GetType().Name}<{typeof(TEvent).Name}>",
            CorrelationId = context.Metadata.CorrelationId?.ToString(),
            TenantId = context.Metadata.TenantId?.ToString()
        };

        // Serialize event payload
        try
        {
            record.RequestBody = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            record.RequestBody = $"{{\"eventType\":\"{typeof(TEvent).Name}\"}}";
        }

        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.HandleAsync(@event, context, cancellationToken);
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
}
