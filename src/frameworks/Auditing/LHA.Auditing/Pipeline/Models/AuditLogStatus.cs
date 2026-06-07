namespace LHA.Auditing.Pipeline;

/// <summary>
/// Status of the audited operation.
/// </summary>
public enum AuditLogStatus : byte
{
    /// <summary>
    /// 0 - Success: Operation completed successfully.
    /// </summary>
    Success = 0,

    /// <summary>
    /// 1 - Failure: Operation failed with an exception.
    /// </summary>
    Failure = 1
}
