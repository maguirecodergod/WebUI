using System.Text;

namespace LHA.BlazorWasm.Components.Table;

// ═══════════════════════════════════════════════════════════════
// REQUEST — sent to server on every data fetch
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Encapsulates paging, sorting, filtering and search state.
/// Passed to <c>ServerDataSource</c> or <c>OnDataRequest</c> callbacks.
/// </summary>
public sealed class DataTableRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public List<SortDefinition> Sorts { get; set; } = [];
    public List<FilterDefinition> Filters { get; set; } = [];

    /// <summary>Serialize to a URL query string for GET endpoints.</summary>
    public string ToQueryString()
    {
        var sb = new StringBuilder();
        sb.Append($"pageNumber={PageNumber}&pageSize={PageSize}");

        if (!string.IsNullOrEmpty(SearchTerm))
            sb.Append($"&search={Uri.EscapeDataString(SearchTerm)}");

        for (var i = 0; i < Sorts.Count; i++)
            sb.Append($"&sort[{i}]={Uri.EscapeDataString(Sorts[i].ToExpression())}");

        for (var i = 0; i < Filters.Count; i++)
        {
            var f = Filters[i];
            sb.Append($"&filter[{i}].field={Uri.EscapeDataString(f.Field)}");
            sb.Append($"&filter[{i}].op={f.Operator}");
            if (f.Value is not null)
                sb.Append($"&filter[{i}].value={Uri.EscapeDataString(f.Value)}");
            if (f.ValueTo is not null)
                sb.Append($"&filter[{i}].valueTo={Uri.EscapeDataString(f.ValueTo)}");
        }

        return sb.ToString();
    }
}

// ═══════════════════════════════════════════════════════════════
// RESPONSE — returned by server data source
// ═══════════════════════════════════════════════════════════════

/// <summary>Server response containing paged items and total count.</summary>
public sealed class DataTableResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
}

// ═══════════════════════════════════════════════════════════════
// SORT — per-column sort state
// ═══════════════════════════════════════════════════════════════

/// <summary>Describes a single sort criterion.</summary>
public sealed record SortDefinition(string Field, SortDirection Direction)
{
    /// <summary>Build "Field desc" or "Field" expression for API.</summary>
    public string ToExpression() =>
        Direction == SortDirection.Descending ? $"{Field} desc" : Field;

    /// <summary>Cycle: None → Asc → Desc → None.</summary>
    public SortDefinition Toggle() => this with
    {
        Direction = Direction switch
        {
            SortDirection.None => SortDirection.Ascending,
            SortDirection.Ascending => SortDirection.Descending,
            _ => SortDirection.None
        }
    };
}

// ═══════════════════════════════════════════════════════════════
// FILTER — per-column filter state
// ═══════════════════════════════════════════════════════════════

/// <summary>Describes a single column filter.</summary>
public sealed class FilterDefinition
{
    public required string Field { get; init; }
    public FilterType Type { get; init; } = FilterType.Text;
    public FilterOperator Operator { get; set; } = FilterOperator.Contains;
    public string? Value { get; set; }
    public string? ValueTo { get; set; }

    public bool IsActive => !string.IsNullOrEmpty(Value);
}

// ═══════════════════════════════════════════════════════════════
// SELECT OPTION — for dropdown / enum filters
// ═══════════════════════════════════════════════════════════════

/// <summary>Label/value pair for Select-type column filters.</summary>
public sealed record SelectOption(string Value, string Label);
