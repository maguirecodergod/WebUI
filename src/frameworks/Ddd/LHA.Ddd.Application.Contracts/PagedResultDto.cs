using System;
using System.Collections.Generic;
using LHA.Ddd.Domain;

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

    /// <inheritdoc />
    public int CurrentPage { get; init; }

    /// <inheritdoc />
    public int TotalPages { get; init; }

    /// <inheritdoc />
    public int PageSize { get; init; }

    public PagedResultDto() { }

    public PagedResultDto(long totalCount, IReadOnlyList<T> items, int pageNumber = 1, int pageSize = 10)
        : base(items)
    {
        TotalCount = totalCount;
        PageSize = pageSize > 0 ? pageSize : 10;
        CurrentPage = pageNumber > 0 ? pageNumber : 1;
        TotalPages = PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
    }

    public PagedResultDto(long totalCount, IReadOnlyList<T> items, PagingParam paging)
        : this(totalCount, items, paging.PageNumber, paging.PageSize)
    {
    }
}
