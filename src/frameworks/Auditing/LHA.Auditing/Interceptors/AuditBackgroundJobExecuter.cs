using System.Diagnostics;
using System.Text.Json;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.BackgroundJob;
using Microsoft.Extensions.Logging;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// Decorator for <see cref="IBackgroundJobExecuter"/> that automatically
/// creates audit log records for every background job execution.
/// <para>
/// Wraps the inner executer via the decorator pattern.
/// Registered in DI to override the default <see cref="BackgroundJobExecuter"/>.
/// </para>
/// </summary>
public sealed class AuditBackgroundJobExecuter : IBackgroundJobExecuter
{
    private readonly IBackgroundJobExecuter _inner;
    private readonly IAuditLogCollector _collector;
    private readonly ILogger<AuditBackgroundJobExecuter> _logger;

    public AuditBackgroundJobExecuter(
        IBackgroundJobExecuter inner,
        IAuditLogCollector collector,
        ILogger<AuditBackgroundJobExecuter> logger)
    {
        _inner = inner;
        _collector = collector;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(JobExecutionContext context)
    {
        var record = new AuditLogRecord
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = AuditActionType.BackgroundJob,
            ActionName = $"BackgroundJob:{context.JobType.Name}"
        };

        // Serialize job args
        try
        {
            record.RequestBody = JsonSerializer.Serialize(context.JobArgs, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            record.RequestBody = $"{{\"jobType\":\"{context.JobType.Name}\"}}";
        }

        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.ExecuteAsync(context);
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
