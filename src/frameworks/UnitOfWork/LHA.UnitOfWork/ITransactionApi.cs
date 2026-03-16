namespace LHA.UnitOfWork;

/// <summary>
/// Represents a database transaction managed by the UnitOfWork.
/// <para>
/// Each <see cref="ITransactionApi"/> instance typically wraps a provider-specific transaction
/// (e.g. <c>DbTransaction</c>, <c>IDbContextTransaction</c>). The UoW commits all registered
/// transactions when <see cref="IUnitOfWork.CompleteAsync"/> succeeds.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may also implement <see cref="ISupportsRollback"/> for explicit rollback support.
/// </remarks>
public interface ITransactionApi : IDisposable
{
    /// <summary>
    /// Commits the underlying transaction.
    /// Called by the UoW after all <see cref="ISupportsSavingChanges.SaveChangesAsync"/> calls succeed.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
