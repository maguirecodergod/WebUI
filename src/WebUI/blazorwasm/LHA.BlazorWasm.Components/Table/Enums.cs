namespace LHA.BlazorWasm.Components.Table;

/// <summary>Client-side (in-memory) or Server-side (API-driven) data source.</summary>
public enum DataTableMode { ClientSide, ServerSide }

/// <summary>Row selection behaviour.</summary>
public enum SelectionMode { None, Single, Multiple }

/// <summary>Column sort direction.</summary>
public enum SortDirection { None, Ascending, Descending }

/// <summary>Cell text alignment.</summary>
public enum ColumnAlignment { Left, Center, Right }

/// <summary>Sticky column position.</summary>
public enum FixedPosition { None, Left, Right }

/// <summary>Per-column filter input type.</summary>
public enum FilterType { Text, Number, NumberRange, Date, DateRange, Select, Boolean }

/// <summary>Filter comparison operator.</summary>
public enum FilterOperator
{
    Contains,
    Equals,
    NotEquals,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Between
}
