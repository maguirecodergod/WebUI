namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Well-known metadata keys for Hangfire-specific data in <see cref="JobContext.SchedulerMetadata"/>.
/// </summary>
public static class HangfireMetadataKeys
{
    /// <summary>The Hangfire background job ID.</summary>
    public const string HangfireJobId = "hangfire.jobId";
}

/// <summary>
/// Extension methods for type-safe access to Hangfire-specific metadata.
/// </summary>
public static class HangfireJobContextExtensions
{
    /// <summary>Gets the Hangfire job ID from the job context.</summary>
    public static string GetHangfireJobId(this JobContext context)
        => context.SchedulerMetadata.TryGetValue(HangfireMetadataKeys.HangfireJobId, out var value)
            ? (string)value
            : string.Empty;
}
