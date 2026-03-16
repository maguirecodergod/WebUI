using System.ComponentModel.DataAnnotations;

namespace LHA.Ddd.Application;

/// <summary>
/// Represents a request that limits the number of results returned.
/// </summary>
public interface ILimitedResultRequest
{
    /// <summary>
    /// Maximum number of items to return.
    /// </summary>
    int MaxResultCount { get; }
}

/// <summary>
/// Represents a paged result request with skip and max count.
/// </summary>
public interface IPagedResultRequest : ILimitedResultRequest
{
    /// <summary>
    /// Number of items to skip before returning results.
    /// </summary>
    int SkipCount { get; }
}

/// <summary>
/// Represents a request that supports sorting.
/// </summary>
public interface ISortedResultRequest
{
    /// <summary>
    /// A sorting expression (e.g. <c>"Name ASC, CreationTime DESC"</c>).
    /// </summary>
    string? Sorting { get; }
}
