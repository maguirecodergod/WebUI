using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Extension methods for applying paging to <see cref="IQueryable{T}"/> sources.
/// </summary>
public static class PagedQueryableExtensions
{
    /// <summary>
    /// Applies paging to the queryable source based on a request object.
    /// </summary>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> source, IPagedResultRequest request)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(request);

        return source.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
    }

    /// <summary>
    /// Applies paging to the queryable source using explicit page number and page size.
    /// </summary>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
