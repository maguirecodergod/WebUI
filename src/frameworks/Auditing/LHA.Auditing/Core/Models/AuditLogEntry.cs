using System.Text;

namespace LHA.Auditing;

/// <summary>
/// Complete audit log entry capturing who did what, when, and the resulting entity changes.
/// This is the primary data structure persisted by <see cref="IAuditingStore"/>.
/// </summary>
public sealed class AuditLogEntry
{
    /// <summary>Application/service name.</summary>
    public string? ApplicationName { get; set; }

    /// <summary>Name of the action (e.g. "Login", "Register").</summary>
    public string? ActionName { get; set; }

    /// <summary>Authenticated user identifier.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Authenticated user display name.</summary>
    public string? UserName { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Tenant display name.</summary>
    public string? TenantName { get; set; }

    /// <summary>Impersonator user identifier (admin-as-user scenarios).</summary>
    public Guid? ImpersonatorUserId { get; set; }

    /// <summary>Impersonator tenant identifier.</summary>
    public Guid? ImpersonatorTenantId { get; set; }

    /// <summary>UTC time of the request/operation start.</summary>
    public DateTimeOffset ExecutionTime { get; set; }

    /// <summary>Total duration in milliseconds.</summary>
    public int ExecutionDuration { get; set; }

    /// <summary>Client/API-key identifier.</summary>
    public string? ClientId { get; set; }

    /// <summary>Distributed correlation identifier.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Client IP address.</summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>HTTP method (GET, POST, etc.).</summary>
    public string? HttpMethod { get; set; }

    /// <summary>HTTP response status code.</summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>Request URL.</summary>
    public string? Url { get; set; }

    /// <summary>Browser/user agent information.</summary>
    public string? BrowserInfo { get; set; }

    /// <summary>Method invocations tracked in this request.</summary>
    public List<AuditLogAction> Actions { get; } = [];

    /// <summary>Exceptions that occurred during processing.</summary>
    public List<Exception> Exceptions { get; } = [];

    /// <summary>Entity changes tracked during this request.</summary>
    public List<EntityChangeEntry> EntityChanges { get; } = [];

    /// <summary>Free-form comments added by contributors.</summary>
    public List<string> Comments { get; } = [];

    /// <summary>Extensible property bag for application-specific data.</summary>
    public Dictionary<string, object?> ExtraProperties { get; } = [];

    /// <summary>
    /// Merges duplicate entity change entries (same entity + type = Updated)
    /// by combining property changes into a single entry.
    /// </summary>
    public void MergeEntityChanges()
    {
        var groups = EntityChanges
            .Where(e => e.ChangeType == CEntityChangeType.Updated)
            .GroupBy(e => (e.EntityTypeFullName, e.EntityId))
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in groups)
        {
            var first = group.First();
            foreach (var duplicate in group.Skip(1))
            {
                first.Merge(duplicate);
                EntityChanges.Remove(duplicate);
            }
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"AUDIT LOG: [{HttpStatusCode?.ToString() ?? "---"}: {(HttpMethod ?? "-------").PadRight(7)}] {Url}");
        sb.AppendLine($"- User     : {UserName} ({UserId})");
        sb.AppendLine($"- Tenant   : {TenantName} ({TenantId})");
        sb.AppendLine($"- Duration : {ExecutionDuration} ms");
        sb.AppendLine($"- Client IP: {ClientIpAddress}");

        if (Actions.Count > 0)
        {
            sb.AppendLine("- Actions:");
            foreach (var action in Actions)
                sb.AppendLine($"  - {action.ServiceName}.{action.MethodName} ({action.ExecutionDuration} ms)");
        }

        if (Exceptions.Count > 0)
        {
            sb.AppendLine("- Exceptions:");
            foreach (var ex in Exceptions)
                sb.AppendLine($"  - {ex.Message}");
        }

        if (EntityChanges.Count > 0)
        {
            sb.AppendLine("- Entity Changes:");
            foreach (var change in EntityChanges)
            {
                sb.AppendLine($"  - [{change.ChangeType}] {change.EntityTypeFullName}, Id = {change.EntityId}");
                foreach (var prop in change.PropertyChanges)
                    sb.AppendLine($"    {prop.PropertyName}: {prop.OriginalValue} -> {prop.NewValue}");
            }
        }

        return sb.ToString();
    }
}
