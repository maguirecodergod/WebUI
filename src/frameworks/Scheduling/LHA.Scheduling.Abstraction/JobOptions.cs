namespace LHA.Scheduling;

/// <summary>
/// Options for a single job execution.
/// </summary>
public sealed class JobOptions
{
    /// <summary>Tenant ID for multi-tenant isolation.</summary>
    public string? TenantId { get; set; }

    /// <summary>Correlation ID for distributed tracing.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>User ID of the actor who enqueued the job.</summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Queue name to dispatch the job to (e.g. "critical", "background", "emails").
    /// If null, the scheduler's default queue is used.
    /// <list type="bullet">
    ///   <item><b>Hangfire:</b> maps to Hangfire queue name.</item>
    ///   <item><b>Quartz:</b> mapped to a trigger group or custom property.</item>
    /// </list>
    /// </summary>
    public string? Queue { get; set; }

    /// <summary>
    /// Max retry attempts for transient failures. If null, the scheduler default is used.
    /// </summary>
    public int? MaxRetries { get; set; }

    /// <summary>
    /// Display name for the job in monitoring dashboards.
    /// If null, derived from the job type name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>Additional metadata passed through to the job context.</summary>
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
