namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Database schema constants for the Audit Log module.
/// </summary>
public static class AuditLogDbConsts
{
    private const string Sep = "_";
    private const string Prefix = "Audit" + Sep;

    public const string AuditLog = Prefix + "Log";
    public const string AuditLogAction = Prefix + "Action";
    public const string EntityChange = Prefix + "EntityChange";
    public const string EntityPropertyChange = Prefix + "PropertyChange";
    public const string AuditLogPipeline = Prefix + "LogPipeline";
}
