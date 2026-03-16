namespace LHA.Core;

/// <summary>
/// Specifies the comparison operator used in dynamic keyword search.
/// </summary>
public enum CSearchOperatorType
{
    /// <summary>Property value contains the keyword.</summary>
    Contains = 0,

    /// <summary>Property value equals the keyword exactly.</summary>
    Equals = 1,

    /// <summary>Property value starts with the keyword.</summary>
    StartsWith = 2
}
