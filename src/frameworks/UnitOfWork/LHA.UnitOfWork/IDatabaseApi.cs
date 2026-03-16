namespace LHA.UnitOfWork;

/// <summary>
/// Marker interface for a database-level API (e.g. EF Core DbContext, Dapper connection).
/// <para>
/// The UnitOfWork maintains a dictionary of <c>string → IDatabaseApi</c> keyed by
/// a provider-specific identifier (e.g. connection string hash, tenant+dbContext type).
/// This allows a single UoW to span multiple databases or multiple DbContext types
/// within the same logical transaction boundary.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may also implement <see cref="ISupportsSavingChanges"/> and/or
/// <see cref="ISupportsRollback"/> to participate in the UoW commit/rollback lifecycle.
/// </remarks>
public interface IDatabaseApi;
