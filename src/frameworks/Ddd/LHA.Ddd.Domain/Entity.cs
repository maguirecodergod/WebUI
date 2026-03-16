namespace LHA.Ddd.Domain;

/// <summary>
/// Base class for entities identified by a single key.
/// Provides identity-based equality semantics.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>>
    where TKey : notnull
{
    /// <inheritdoc />
    public virtual TKey Id { get; protected init; } = default!;

    /// <inheritdoc />
    public object[] GetKeys() => [Id];

    /// <inheritdoc />
    public bool Equals(Entity<TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;

        // Transient entities (default key) are never equal unless same reference.
        if (EqualityComparer<TKey>.Default.Equals(Id, default!)) return false;

        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Entity<TKey>);

    /// <inheritdoc />
    public override int GetHashCode() =>
        EqualityComparer<TKey>.Default.Equals(Id, default!)
            ? base.GetHashCode()
            : HashCode.Combine(GetType(), Id);

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name} (Id = {Id})";

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right) => Equals(left, right);
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right) => !Equals(left, right);
}
