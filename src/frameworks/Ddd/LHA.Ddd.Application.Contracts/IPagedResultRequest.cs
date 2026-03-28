using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Represents a request that supports paging.
/// </summary>
public interface IPagedResultRequest
{
    /// <summary>Current page number (1-based).</summary>
    int PageNumber { get; }

    /// <summary>Number of items per page.</summary>
    int PageSize { get; }
}

/// <summary>
/// Represents a request that supports sorting.
/// </summary>
public interface ISortedResultRequest
{
    /// <summary>Sorting parameters.</summary>
    SorterParam? Sorter { get; }
}
