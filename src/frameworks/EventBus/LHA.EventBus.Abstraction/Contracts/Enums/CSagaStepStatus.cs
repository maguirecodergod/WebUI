namespace LHA.EventBus
{
    /// <summary>
    /// Represents the outcome of executing a single saga step.
    /// </summary>
    public enum CSagaStepStatus
    {
        /// <summary>
        /// 1 - Pending: Step has not been executed yet.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// 2 - Completed: Step completed successfully.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// 3 - Failed: Step failed and may need compensation.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// 4 - Compensated: Step was compensated (rolled back).
        /// </summary>
        Compensated = 4
    }
}