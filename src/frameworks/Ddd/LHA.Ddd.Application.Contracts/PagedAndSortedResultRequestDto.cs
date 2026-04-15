namespace LHA.Ddd.Application;

/// <summary>
/// DTO for paged and sorted requests.
/// </summary>
public class PagedAndSortedResultRequestDto : PagedResultRequestDto, ISortedResultRequest
{
    // Inherits Sorter from PagedResultRequestDto, matches ISortedResultRequest.
}

/// <summary>
/// Generic version of <see cref="PagedAndSortedResultRequestDto"/> that includes a filter.
/// </summary>
/// <typeparam name="T">The filter object type.</typeparam>
public class PagedAndSortedResultRequestDto<TFilter> : PagedAndSortedResultRequestDto
{
    /// <summary>Filter object for structured queries.</summary>
    public TFilter? Filter { get; set; }
}
