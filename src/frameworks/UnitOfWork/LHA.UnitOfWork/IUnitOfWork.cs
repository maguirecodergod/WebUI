namespace LHA.UnitOfWork;

/// <summary>
/// Represents a logical unit of work that may span one or more databases and transactions.
/// <para>
/// <b>Lifecycle:</b> Create via <see cref="IUnitOfWorkManager.Begin"/>,
/// perform work, call <see cref="CompleteAsync"/>, and <see cref="Dispose"/>.
/// Failure to call <see cref="CompleteAsync"/> before disposal triggers rollback.
/// </para>
/// <para>
/// <b>Distributed databases:</b> The UoW maintains separate <see cref="IDatabaseApi"/>
/// and <see cref="ITransactionApi"/> registrations keyed by a provider-specific identifier,
/// allowing a single logical UoW to coordinate writes across multiple databases.
/// </para>
/// </summary>
/// <example>
/// <code>
/// await using var uow = unitOfWorkManager.Begin(new UnitOfWorkOptions { IsTransactional = true });
/// // ... repository operations ...
/// await uow.CompleteAsync();
/// </code>
/// </example>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>Unique identifier for this UoW instance.</summary>
    Guid Id { get; }

    /// <summary>The options this UoW was initialized with.</summary>
    UnitOfWorkOptions Options { get; }

    /// <summary>
    /// The outer (parent) UoW when nested UoWs are used.
    /// <c>null</c> if this is the outermost UoW.
    /// </summary>
    IUnitOfWork? Outer { get; }

    /// <summary>Whether this UoW has been reserved but not yet initialized.</summary>
    bool IsReserved { get; }

    /// <summary>Whether this UoW has been disposed.</summary>
    bool IsDisposed { get; }

    /// <summary>Whether <see cref="CompleteAsync"/> has been called successfully.</summary>
    bool IsCompleted { get; }

    /// <summary>Reservation name (non-null only when <see cref="IsReserved"/> is true).</summary>
    string? ReservationName { get; }

    /// <summary>Arbitrary key-value bag for providers to attach contextual data.</summary>
    Dictionary<string, object> Items { get; }

    /// <summary>The <see cref="IServiceProvider"/> scoped to this UoW.</summary>
    IServiceProvider ServiceProvider { get; }

    // ─── Lifecycle ───────────────────────────────────────────────────

    /// <summary>Raised when the UoW fails (either exception during Complete or disposal without Complete).</summary>
    event EventHandler<UnitOfWorkFailedEventArgs> Failed;

    /// <summary>Raised when the UoW is disposed.</summary>
    event EventHandler<UnitOfWorkEventArgs> Disposed;

    /// <summary>Links this UoW to an outer (parent) UoW for nesting.</summary>
    void SetOuter(IUnitOfWork? outer);

    /// <summary>Initializes this UoW with the given options. Can only be called once.</summary>
    void Initialize(UnitOfWorkOptions options);

    /// <summary>Reserves this UoW for later initialization (used by middleware patterns).</summary>
    void Reserve(string reservationName);

    /// <summary>
    /// Flushes all pending changes across all registered <see cref="IDatabaseApi"/>s
    /// without committing transactions.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes the UoW: saves changes, publishes domain events, commits transactions,
    /// and invokes completion handlers.
    /// </summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);

    /// <summary>Explicitly rolls back all registered databases and transactions.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>Registers a callback invoked after successful completion.</summary>
    void OnCompleted(Func<Task> handler);

    // ─── Domain Events ───────────────────────────────────────────────

    /// <summary>Enqueues a local (in-process) domain event to be published on commit.</summary>
    void AddLocalEvent(UnitOfWorkEventRecord eventRecord);

    /// <summary>Enqueues a distributed domain event to be published on commit (e.g. via outbox).</summary>
    void AddDistributedEvent(UnitOfWorkEventRecord eventRecord);

    // ─── Database / Transaction Registry ─────────────────────────────

    /// <summary>Finds a registered database API by key, or <c>null</c>.</summary>
    IDatabaseApi? FindDatabaseApi(string key);

    /// <summary>Registers a database API. Throws if the key already exists.</summary>
    void AddDatabaseApi(string key, IDatabaseApi api);

    /// <summary>Gets an existing database API by key, or creates and registers one using the factory.</summary>
    IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory);

    /// <summary>Finds a registered transaction API by key, or <c>null</c>.</summary>
    ITransactionApi? FindTransactionApi(string key);

    /// <summary>Registers a transaction API. Throws if the key already exists.</summary>
    void AddTransactionApi(string key, ITransactionApi api);

    /// <summary>Gets an existing transaction API by key, or creates and registers one using the factory.</summary>
    ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory);
}
