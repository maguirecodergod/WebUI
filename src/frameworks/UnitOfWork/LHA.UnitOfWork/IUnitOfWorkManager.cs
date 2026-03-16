namespace LHA.UnitOfWork;

/// <summary>
/// Manages the lifecycle of <see cref="IUnitOfWork"/> instances.
/// <para>
/// Handles ambient (AsyncLocal) UoW tracking, nesting (child UoWs),
/// and the reservation pattern (reserve → begin later from middleware).
/// </para>
/// </summary>
public interface IUnitOfWorkManager
{
    /// <summary>
    /// The current active (non-reserved, non-disposed, non-completed) UoW, or <c>null</c>.
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// Begins a new unit of work.
    /// If there is already an active UoW and <paramref name="requiresNew"/> is <c>false</c>,
    /// returns a <see cref="ChildUnitOfWork"/> that delegates to the parent.
    /// </summary>
    /// <param name="options">Options for the new UoW.</param>
    /// <param name="requiresNew">
    /// If <c>true</c>, always creates a new root UoW even if one already exists.
    /// </param>
    IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false);

    /// <summary>
    /// Reserves a UoW for later initialization. Used by middleware to establish a UoW
    /// boundary early, then <see cref="BeginReserved"/> initializes it later.
    /// </summary>
    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    /// <summary>
    /// Initializes a previously reserved UoW with the given options.
    /// Throws if no reservation with the given name exists.
    /// </summary>
    void BeginReserved(string reservationName, UnitOfWorkOptions options);

    /// <summary>
    /// Tries to initialize a previously reserved UoW. Returns <c>false</c> if not found.
    /// </summary>
    bool TryBeginReserved(string reservationName, UnitOfWorkOptions options);
}
