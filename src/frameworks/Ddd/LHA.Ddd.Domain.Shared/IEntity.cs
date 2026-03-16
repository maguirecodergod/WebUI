namespace LHA.Ddd.Domain;

/// <summary>
/// Marker interface for all domain entities.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Returns the composite key values that uniquely identify this entity.
    /// </summary>
    object[] GetKeys();
}

/// <summary>
/// Marker interface for domain entities identified by a single key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public interface IEntity<out TKey> : IEntity where TKey : notnull
{
    /// <summary>
    /// Unique identifier of the entity.
    /// </summary>
    TKey Id { get; }
}
