using System.Collections.Generic;
using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Base DTO for paged requests.
/// </summary>
public class PagedResultRequestDto : PagingParam, IPagedResultRequest
{
    /// <summary>Simple full-text search query.</summary>
    public string? SearchQuery { get; set; }

    /// <summary>Columns to apply search to.</summary>
    public List<string> AllowSearchColumns { get; set; } = new List<string>();

    /// <summary>Sorting parameters.</summary>
    public SorterParam? Sorter { get; set; }
}
