using LHA.Ddd.Domain;

namespace LHA.Ddd.Application;

/// <summary>
/// Base DTO for entities identified by a single key.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class EntityDto<TKey> where TKey : notnull
{
    /// <summary>
    /// The entity identifier.
    /// </summary>
    public required TKey Id { get; init; }
}
