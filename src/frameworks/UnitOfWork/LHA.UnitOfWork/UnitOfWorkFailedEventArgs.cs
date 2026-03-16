namespace LHA.UnitOfWork;

/// <summary>
/// Event arguments raised when a <see cref="IUnitOfWork"/> fails.
/// </summary>
public sealed class UnitOfWorkFailedEventArgs : UnitOfWorkEventArgs
{
    /// <summary>
    /// The exception that caused the failure, or <c>null</c> if the UoW was simply
    /// disposed without calling <see cref="IUnitOfWork.CompleteAsync"/>.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary><c>true</c> if <see cref="IUnitOfWork.RollbackAsync"/> was called explicitly.</summary>
    public bool IsRolledBack { get; }

    public UnitOfWorkFailedEventArgs(IUnitOfWork unitOfWork, Exception? exception, bool isRolledBack)
        : base(unitOfWork)
    {
        Exception = exception;
        IsRolledBack = isRolledBack;
    }
}
