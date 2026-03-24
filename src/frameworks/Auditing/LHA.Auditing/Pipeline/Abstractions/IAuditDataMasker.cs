namespace LHA.Auditing.Pipeline;

/// <summary>
/// Masks sensitive data in audit log records.
/// Applied automatically by <see cref="IAuditLogCollector"/> before buffering.
/// </summary>
public interface IAuditDataMasker
{
    /// <summary>
    /// Masks sensitive values in the record's request body, response body, and tags.
    /// </summary>
    string? MaskJson(string? json);
}
