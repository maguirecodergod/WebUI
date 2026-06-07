namespace LHA.Core;

/// <summary>
/// Specifies how multiple search keywords are combined.
/// </summary>
public enum CSearchCombineModeType
{
    /// <summary>
    /// 0 - Or: Any keyword must match at least one column (logical OR across keywords).
    /// </summary>
    Or = 0,

    /// <summary>
    /// 1 - And: Every keyword must match at least one column (logical AND across keywords).
    /// </summary>
    And = 1
}
