using System.Collections.Generic;

namespace LHA.Ddd.Domain;

/// <summary>
/// Combined paging, filtering, and searching parameters for table-based UI.
/// </summary>
/// <typeparam name="T">The filter object type.</typeparam>
public class TableParam<T> : PagingParam
{
    /// <summary>The search query string.</summary>
    public string SearchQuery { get; set; } = string.Empty;
    
    /// <summary>Columns allowed for full-text search.</summary>
    public List<string> AllowSearchColumns { get; set; } = new List<string>();
    
    /// <summary>The filter object.</summary>
    public T? Filter { get; set; }
    
    /// <summary>The sorting parameters.</summary>
    public SorterParam? Sorter { get; set; } = null;
}
