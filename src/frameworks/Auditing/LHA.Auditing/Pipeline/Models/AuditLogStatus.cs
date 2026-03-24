namespace LHA.Auditing.Pipeline;

/// <summary>
/// Status of the audited operation.
/// </summary>
public enum AuditLogStatus : byte
{
    /// <summary>Operation completed successfully.</summary>
    Success = 0,

    /// <summary>Operation failed with an exception.</summary>
    Failure = 1
}
