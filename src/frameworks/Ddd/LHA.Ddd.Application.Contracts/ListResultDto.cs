namespace LHA.Ddd.Application;

/// <summary>
/// Default implementation of <see cref="IListResult{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class ListResultDto<T> : IListResult<T>
{
    /// <inheritdoc />
    public IReadOnlyList<T> Items { get; init; } = [];

    public ListResultDto() { }

    public ListResultDto(IReadOnlyList<T> items)
    {
        Items = items;
    }
}
