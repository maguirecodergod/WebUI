using LHA.Ddd.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Extends <see cref="IRepository{TEntity,TKey}"/> with EF Core–specific members
/// for advanced scenarios requiring direct <see cref="DbContext"/> or <see cref="DbSet{TEntity}"/> access.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
public interface IEfCoreRepository<TEntity, in TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// Gets the underlying <see cref="DbContext"/> for this repository.
    /// </summary>
    Task<DbContext> GetDbContextAsync();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for direct query access.
    /// </summary>
    Task<DbSet<TEntity>> GetDbSetAsync();
}
