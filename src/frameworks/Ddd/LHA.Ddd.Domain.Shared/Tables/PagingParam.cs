namespace LHA.Ddd.Domain;

/// <summary>
/// Basic paging parameters.
/// </summary>
public class PagingParam
{
    private const int MaxPageSize = 50;
    
    /// <summary>The page number (1-based).</summary>
    public int PageNumber { get; set; } = 1;
    
    private int _pageSize = 10;
    
    /// <summary>The number of items per page.</summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
