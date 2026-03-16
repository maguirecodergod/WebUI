using Hangfire;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Configuration options for the LHA Hangfire Scheduling integration.
/// </summary>
public sealed class HangfireSchedulingOptions
{
    /// <summary>
    /// Default queue name for jobs that don't specify a queue.
    /// Hangfire default is "default".
    /// </summary>
    public string DefaultQueue { get; set; } = "default";

    /// <summary>
    /// Default max retry attempts for failed jobs.
    /// Hangfire global default is 10.
    /// </summary>
    public int DefaultMaxRetries { get; set; } = 10;

    /// <summary>
    /// Worker count (number of concurrent job processing threads).
    /// If null, Hangfire uses Environment.ProcessorCount * 5.
    /// </summary>
    public int? WorkerCount { get; set; }

    /// <summary>
    /// Queues to listen on (ordered by priority, first = highest).
    /// If null, listens on <see cref="DefaultQueue"/> only.
    /// </summary>
    public string[]? Queues { get; set; }

    /// <summary>
    /// Hangfire dashboard path (e.g. "/hangfire").
    /// Set to null to disable the dashboard.
    /// </summary>
    public string? DashboardPath { get; set; } = "/hangfire";

    /// <summary>
    /// Callback to configure the underlying Hangfire GlobalConfiguration.
    /// Use this to set up storage (SQL Server, PostgreSQL, Redis, in-memory).
    /// This is REQUIRED — there is no default storage.
    /// <example>
    /// <code>
    /// options.ConfigureHangfire = config =>
    ///     config.UsePostgreSqlStorage(connectionString);
    /// </code>
    /// </example>
    /// </summary>
    public Action<IGlobalConfiguration>? ConfigureHangfire { get; set; }

    /// <summary>
    /// If true, the Hangfire server (background processing) is started.
    /// Set to false for web-only nodes that only enqueue jobs but don't process them.
    /// </summary>
    public bool EnableServer { get; set; } = true;
}
