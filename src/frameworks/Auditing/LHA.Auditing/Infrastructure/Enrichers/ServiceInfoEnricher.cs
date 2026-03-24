using LHA.Auditing.Pipeline;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Enriches audit log records with service name and instance ID.
/// </summary>
internal sealed class ServiceInfoEnricher : IAuditLogEnricher
{
    private readonly string _serviceName;
    private readonly string _instanceId;

    public ServiceInfoEnricher(IOptions<AuditPipelineOptions> options)
    {
        _serviceName = options.Value.ServiceName;
        _instanceId = options.Value.InstanceId ?? Environment.MachineName;
    }

    public void Enrich(AuditLogRecord record)
    {
        record.ServiceName ??= _serviceName;
        record.InstanceId ??= _instanceId;
    }
}
