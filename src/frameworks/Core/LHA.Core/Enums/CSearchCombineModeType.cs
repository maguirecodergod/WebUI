namespace LHA.Core;

/// <summary>
/// Specifies how multiple search keywords are combined.
/// </summary>
public enum CSearchCombineModeType
{
    /// <summary>
    /// Any keyword must match at least one column (logical OR across keywords).
    /// </summary>
    Or = 0,

    /// <summary>
    /// Every keyword must match at least one column (logical AND across keywords).
    /// </summary>
    And = 1
}
