namespace LHA.Ddd.Application;

/// <summary>
/// DTO for paged and sorted requests.
/// </summary>
public class PagedAndSortedResultRequestDto : PagedResultRequestDto, ISortedResultRequest
{
    /// <inheritdoc />
    public string? Sorting { get; init; }
}
