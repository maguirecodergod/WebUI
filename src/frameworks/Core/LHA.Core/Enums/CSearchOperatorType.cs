namespace LHA.Core;

/// <summary>
/// Specifies the comparison operator used in dynamic keyword search.
/// </summary>
public enum CSearchOperatorType
{
    /// <summary>
    /// 0 - Contains: Property value contains the keyword.
    /// </summary>
    Contains = 0,

    /// <summary>
    /// 1 - Equals: Property value equals the keyword exactly.
    /// </summary>
    Equals = 1,

    /// <summary>
    /// 2 - StartsWith: Property value starts with the keyword.
    /// </summary>
    StartsWith = 2
}
