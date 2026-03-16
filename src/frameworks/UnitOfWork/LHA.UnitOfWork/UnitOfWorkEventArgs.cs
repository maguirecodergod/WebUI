namespace LHA.UnitOfWork;

/// <summary>
/// Event arguments for <see cref="IUnitOfWork"/> lifecycle events.
/// </summary>
public class UnitOfWorkEventArgs : EventArgs
{
    /// <summary>The unit of work that raised this event.</summary>
    public IUnitOfWork UnitOfWork { get; }

    public UnitOfWorkEventArgs(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        UnitOfWork = unitOfWork;
    }
}
