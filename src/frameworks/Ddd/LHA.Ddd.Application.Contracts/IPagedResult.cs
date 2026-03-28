namespace LHA.Ddd.Application;

/// <summary>
/// Represents a paged subset of a larger result set.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IPagedResult<out T> : IListResult<T>
{
    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    long TotalCount { get; }

    /// <summary>Current page number (1-based).</summary>
    int CurrentPage { get; }

    /// <summary>Total number of pages.</summary>
    int TotalPages { get; }

    /// <summary>Number of items per page.</summary>
    int PageSize { get; }
}
