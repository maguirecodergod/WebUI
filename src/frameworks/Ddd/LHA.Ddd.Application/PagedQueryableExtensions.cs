namespace LHA.Ddd.Application;

/// <summary>
/// Extension methods for applying paging to <see cref="IQueryable{T}"/> sources.
/// </summary>
public static class PagedQueryableExtensions
{
    /// <summary>
    /// Applies skip/take paging to the queryable source using the specified request parameters.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The queryable source.</param>
    /// <param name="request">The paged result request containing skip and max count.</param>
    /// <returns>The paged queryable.</returns>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> source, IPagedResultRequest request)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(request);

        return source.Skip(request.SkipCount).Take(request.MaxResultCount);
    }

    /// <summary>
    /// Applies skip/take paging to the queryable source.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The queryable source.</param>
    /// <param name="skipCount">Number of items to skip.</param>
    /// <param name="maxResultCount">Maximum number of items to return.</param>
    /// <returns>The paged queryable.</returns>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> source, int skipCount, int maxResultCount)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Skip(skipCount).Take(maxResultCount);
    }
}
