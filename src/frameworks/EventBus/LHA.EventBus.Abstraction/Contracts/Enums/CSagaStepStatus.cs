namespace LHA.EventBus
{
    /// <summary>
    /// Represents the outcome of executing a single saga step.
    /// </summary>
    public enum CSagaStepStatus
    {
        /// <summary>Step has not been executed yet.</summary>
        Pending = 1,

        /// <summary>Step completed successfully.</summary>
        Completed = 2,

        /// <summary>Step failed and may need compensation.</summary>
        Failed = 3,

        /// <summary>Step was compensated (rolled back).</summary>
        Compensated = 4
    }
}