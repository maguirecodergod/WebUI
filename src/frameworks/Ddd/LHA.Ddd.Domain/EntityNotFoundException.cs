namespace LHA.Ddd.Domain;

/// <summary>
/// Exception thrown when an entity with the specified identifier cannot be found.
/// </summary>
public sealed class EntityNotFoundException : Exception
{
    /// <summary>
    /// The type of the entity that was not found.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// The identifier value that was looked up.
    /// </summary>
    public object? Id { get; }

    public EntityNotFoundException(Type entityType, object? id)
        : base($"Entity of type '{entityType.Name}' with id '{id}' was not found.")
    {
        EntityType = entityType;
        Id = id;
    }

    public EntityNotFoundException(Type entityType, object? id, Exception? innerException)
        : base($"Entity of type '{entityType.Name}' with id '{id}' was not found.", innerException)
    {
        EntityType = entityType;
        Id = id;
    }
}
