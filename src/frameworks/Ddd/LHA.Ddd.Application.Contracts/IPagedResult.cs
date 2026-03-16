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
}
