using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Base DTO for paged requests.
/// </summary>
public class PagedResultRequestDto : PagingParam, IPagedResultRequest
{
    /// <summary>Simple full-text search query.</summary>
    public string? SearchQuery { get; set; }

    /// <summary>Columns to apply search to. Using array for better Minimal API binding.</summary>
    public string[] AllowSearchColumns { get; set; } = [];

    /// <summary>Sorting property name.</summary>
    public string? SorterKey { get; set; }

    /// <summary>True for ascending, false for descending.</summary>
    public bool? SorterIsAsc { get; set; }

    /// <summary>
    /// Derived Sorter object for backend processing. 
    /// Get-only properties are ignored by Minimal API parameter binding.
    /// </summary>
    public SorterParam? Sorter => SorterKey != null 
        ? new SorterParam { KeyName = SorterKey, IsASC = SorterIsAsc ?? true } 
        : null;
}
