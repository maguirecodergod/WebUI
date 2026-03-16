namespace LHA.UnitOfWork;

/// <summary>
/// Indicates that a <see cref="IDatabaseApi"/> or <see cref="ITransactionApi"/>
/// supports explicit rollback.
/// </summary>
public interface ISupportsRollback
{
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
