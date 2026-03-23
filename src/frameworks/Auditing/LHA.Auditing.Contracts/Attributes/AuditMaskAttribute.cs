namespace LHA.Auditing;

/// <summary>
/// Marks a property to be masked in audit log serialization.
/// The property value will be replaced with the specified mask string.
/// <para>
/// Use for fields that should appear in audit logs but with redacted values
/// (e.g., passwords → "****", credit cards → "****1234").
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public sealed class AuditMaskAttribute : Attribute
{
    /// <summary>
    /// The mask string to replace the value with.
    /// Default: "****".
    /// </summary>
    public string Mask { get; }

    /// <summary>
    /// If true, preserves the last N characters of the original value.
    /// Useful for credit card numbers (show last 4 digits).
    /// Default: 0 (full mask).
    /// </summary>
    public int PreserveSuffix { get; }

    public AuditMaskAttribute(string mask = "****", int preserveSuffix = 0)
    {
        Mask = mask;
        PreserveSuffix = preserveSuffix;
    }
}
