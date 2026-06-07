namespace LHA.BlazorWasm.Components.Table;

/// <summary>Client-side (in-memory) or Server-side (API-driven) data source.</summary>
public enum CDataTableMode { ClientSide, ServerSide }

/// <summary>Row selection behaviour.</summary>
public enum CSelectionMode { None, Single, Multiple }

/// <summary>Column sort direction.</summary>
public enum CSortDirection { None, Ascending, Descending }

/// <summary>Cell text alignment.</summary>
public enum CColumnAlignment { Left, Center, Right }

/// <summary>Sticky column position.</summary>
public enum CFixedPosition { None, Left, Right }

/// <summary>Per-column filter input type.</summary>
public enum CFilterType { Text, Number, NumberRange, Date, DateRange, Select, Boolean }

/// <summary>Filter comparison operator.</summary>
public enum CFilterOperator
{
    /// <summary>
    /// 0 - Contains
    /// </summary>
    Contains,
    /// <summary>
    /// 1 - Equals
    /// </summary>
    Equals,
    /// <summary>
    /// 2 - NotEquals
    /// </summary>
    NotEquals,
    /// <summary>
    /// 3 - StartsWith
    /// </summary>
    StartsWith,
    /// <summary>
    /// 4 - EndsWith
    /// </summary>
    EndsWith,
    /// <summary>
    /// 5 - GreaterThan
    /// </summary>
    GreaterThan,
    /// <summary>
    /// 6 - GreaterThanOrEqual
    /// </summary>
    GreaterThanOrEqual,
    /// <summary>
    /// 7 - LessThan
    /// </summary>
    LessThan,
    /// <summary>
    /// 8 - LessThanOrEqual
    /// </summary>
    LessThanOrEqual,
    /// <summary>
    /// 9 - Between
    /// </summary>
    Between
}
