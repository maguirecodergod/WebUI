namespace LHA.Ddd.Application;

/// <summary>
/// Default implementation of <see cref="IPagedResult{T}"/>.
/// Contains items, total count, and computed paging metadata.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>
{
    /// <inheritdoc />
    public long TotalCount { get; init; }

    /// <summary>Current page number (1-based).</summary>
    public int CurrentPage { get; init; }

    /// <summary>Total number of pages.</summary>
    public int TotalPages { get; init; }

    /// <summary>Number of items per page.</summary>
    public int PageSize { get; init; }

    public PagedResultDto() { }

    public PagedResultDto(long totalCount, IReadOnlyList<T> items, int skipCount = 0, int maxResultCount = 10)
        : base(items)
    {
        // Reconcile: separate COUNT and SELECT queries may see different snapshots
        // under non-transactional reads. When items.Count < maxResultCount we know
        // we are on the last page — the actual total is skipCount + items.Count.
        // When items fill the page, trust the DB count but ensure it's at least
        // skipCount + items.Count to avoid impossible paging states.
        TotalCount = items.Count < maxResultCount
            ? skipCount + items.Count
            : Math.Max(totalCount, skipCount + items.Count);

        PageSize = maxResultCount > 0 ? maxResultCount : 10;
        CurrentPage = PageSize > 0 ? (skipCount / PageSize) + 1 : 1;
        TotalPages = PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
    }
}
