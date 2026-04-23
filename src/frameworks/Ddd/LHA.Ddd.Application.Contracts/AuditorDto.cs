namespace LHA.Ddd.Application;

/// <summary>
/// Represents the user who performed an action (creation, modification, deletion) for auditing purposes.
/// </summary>
public class AuditorDto
{
    /// <summary>
    /// Identifier of the auditor.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// The name of the auditor.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The avatar of the auditor.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// The email of the auditor.
    /// </summary>
    public string? Email { get; set; }
}
