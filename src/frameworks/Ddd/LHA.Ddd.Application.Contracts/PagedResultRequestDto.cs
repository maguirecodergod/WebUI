using System.ComponentModel.DataAnnotations;

namespace LHA.Ddd.Application;

/// <summary>
/// Base DTO for paged requests with validation.
/// </summary>
public class PagedResultRequestDto : IPagedResultRequest
{
    /// <summary>
    /// Default maximum result count when not specified.
    /// </summary>
    public const int DefaultMaxResultCount = 10;

    /// <summary>
    /// Hard upper limit for <see cref="MaxResultCount"/>.
    /// </summary>
    public const int MaxMaxResultCount = 1000;

    /// <inheritdoc />
    [Range(0, int.MaxValue)]
    public int SkipCount { get; set; }

    /// <inheritdoc />
    [Range(1, MaxMaxResultCount)]
    public int MaxResultCount { get; set; } = DefaultMaxResultCount;
}
