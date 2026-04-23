namespace LHA.Ddd.Application;

/// <summary>
/// Represents an audit event (creation, modification, deletion) containing the time and the actor.
/// </summary>
public class AuditActionDto
{
    /// <summary>
    /// The time when the action occurred.
    /// </summary>
    public DateTimeOffset? Time { get; set; }

    /// <summary>
    /// Detailed information about the actor who performed the action.
    /// </summary>
    public AuditorDto? Actor { get; set; }
}
