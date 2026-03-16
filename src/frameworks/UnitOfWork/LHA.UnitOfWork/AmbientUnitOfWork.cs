namespace LHA.UnitOfWork;

/// <summary>
/// Tracks the ambient (current) <see cref="IUnitOfWork"/> via <see cref="AsyncLocal{T}"/>.
/// <para>
/// This ensures each async execution context has its own UoW reference,
/// which is critical for correct behaviour in concurrent web request scenarios.
/// </para>
/// </summary>
public sealed class AmbientUnitOfWork
{
    private readonly AsyncLocal<IUnitOfWork?> _currentUow = new();

    /// <summary>The raw current UoW (may be reserved, disposed, or completed).</summary>
    public IUnitOfWork? UnitOfWork => _currentUow.Value;

    /// <summary>Sets the ambient UoW for the current async context.</summary>
    public void SetUnitOfWork(IUnitOfWork? unitOfWork) => _currentUow.Value = unitOfWork;

    /// <summary>
    /// Returns the current *active* UoW, skipping any that are reserved, disposed, or completed.
    /// This walks the <see cref="IUnitOfWork.Outer"/> chain until an active UoW is found.
    /// </summary>
    public IUnitOfWork? GetCurrentByChecking()
    {
        var uow = UnitOfWork;

        while (uow is not null && (uow.IsReserved || uow.IsDisposed || uow.IsCompleted))
        {
            uow = uow.Outer;
        }

        return uow;
    }
}
