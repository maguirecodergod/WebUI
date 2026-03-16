namespace LHA.BackgroundJob;

/// <summary>
/// Maps a background job arguments type to the job implementation type and name.
/// </summary>
public sealed class BackgroundJobConfiguration
{
    /// <summary>The job arguments type.</summary>
    public Type ArgsType { get; }

    /// <summary>The job implementation type (implements <see cref="IBackgroundJob{TArgs}"/>).</summary>
    public Type JobType { get; }

    /// <summary>The job name (used for serialization and routing).</summary>
    public string JobName { get; }

    public BackgroundJobConfiguration(Type jobType, Type argsType, string jobName)
    {
        ArgumentNullException.ThrowIfNull(jobType);
        ArgumentNullException.ThrowIfNull(argsType);
        ArgumentException.ThrowIfNullOrWhiteSpace(jobName);

        JobType = jobType;
        ArgsType = argsType;
        JobName = jobName;
    }

    /// <summary>
    /// Extracts the <c>TArgs</c> type from a job type implementing
    /// <see cref="IBackgroundJob{TArgs}"/>.
    /// </summary>
    public static Type GetArgsType(Type jobType)
    {
        ArgumentNullException.ThrowIfNull(jobType);

        foreach (var iface in jobType.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IBackgroundJob<>))
            {
                return iface.GetGenericArguments()[0];
            }
        }

        throw new InvalidOperationException(
            $"Type {jobType.FullName} does not implement {typeof(IBackgroundJob<>).FullName}.");
    }
}
