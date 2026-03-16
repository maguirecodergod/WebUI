namespace LHA.Scheduling.Quartz;

/// <summary>
/// Well-known keys used in Quartz <see cref="global::Quartz.JobDataMap"/> to pass
/// LHA scheduling context between the scheduler and the job adapter.
/// </summary>
internal static class QuartzDataMapKeys
{
    public const string SerializedParameters = "lha.serializedParameters";
    public const string TenantId = "lha.tenantId";
    public const string CorrelationId = "lha.correlationId";
    public const string UserId = "lha.userId";
    public const string MaxRetries = "lha.maxRetries";
    public const string RetryAttempt = "lha.retryAttempt";
    public const string JobTypeName = "lha.jobType";
}

/// <summary>
/// Well-known metadata keys for Quartz-specific data in <see cref="JobContext.SchedulerMetadata"/>.
/// </summary>
public static class QuartzMetadataKeys
{
    /// <summary>The Quartz fire-instance ID for the current execution.</summary>
    public const string FireInstanceId = "quartz.fireInstanceId";

    /// <summary>The full <c>group.name</c> job key.</summary>
    public const string JobKey = "quartz.jobKey";

    /// <summary>The full <c>group.name</c> trigger key.</summary>
    public const string TriggerKey = "quartz.triggerKey";

    /// <summary>The originally scheduled fire time (ISO 8601 UTC).</summary>
    public const string ScheduledFireTimeUtc = "quartz.scheduledFireTimeUtc";
}

/// <summary>
/// Extension methods for type-safe access to Quartz-specific metadata
/// in <see cref="JobContext.SchedulerMetadata"/>.
/// </summary>
public static class QuartzJobContextExtensions
{
    /// <summary>Gets the Quartz fire-instance ID from the job context.</summary>
    public static string GetQuartzFireInstanceId(this JobContext context)
        => context.SchedulerMetadata.TryGetValue(QuartzMetadataKeys.FireInstanceId, out var value)
            ? (string)value
            : string.Empty;

    /// <summary>Gets the Quartz job key from the job context.</summary>
    public static string GetQuartzJobKey(this JobContext context)
        => context.SchedulerMetadata.TryGetValue(QuartzMetadataKeys.JobKey, out var value)
            ? (string)value
            : string.Empty;

    /// <summary>Gets the Quartz trigger key from the job context.</summary>
    public static string GetQuartzTriggerKey(this JobContext context)
        => context.SchedulerMetadata.TryGetValue(QuartzMetadataKeys.TriggerKey, out var value)
            ? (string)value
            : string.Empty;
}
