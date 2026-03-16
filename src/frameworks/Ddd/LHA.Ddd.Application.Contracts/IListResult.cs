namespace LHA.Ddd.Application;

/// <summary>
/// Represents a list result returned from an application service.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IListResult<out T>
{
    /// <summary>
    /// The list of items.
    /// </summary>
    IReadOnlyList<T> Items { get; }
}
