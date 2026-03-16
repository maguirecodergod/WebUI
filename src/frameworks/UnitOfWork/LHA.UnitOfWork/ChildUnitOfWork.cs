namespace LHA.UnitOfWork;

/// <summary>
/// A child (nested) unit of work that delegates all operations to its parent.
/// <para>
/// <see cref="CompleteAsync"/> is a no-op — only the outermost UoW commits.
/// <see cref="Dispose"/> is a no-op — the parent manages the actual lifetime.
/// </para>
/// </summary>
internal sealed class ChildUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _parent;

    public ChildUnitOfWork(IUnitOfWork parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        _parent = parent;

        // Forward parent events
        _parent.Failed += (sender, args) => Failed?.Invoke(sender!, args);
        _parent.Disposed += (sender, args) => Disposed?.Invoke(sender!, args);
    }

    // ─── Proxied properties ──────────────────────────────────────────

    public Guid Id => _parent.Id;
    public UnitOfWorkOptions Options => _parent.Options;
    public IUnitOfWork? Outer => _parent.Outer;
    public bool IsReserved => _parent.IsReserved;
    public bool IsDisposed => _parent.IsDisposed;
    public bool IsCompleted => _parent.IsCompleted;
    public string? ReservationName => _parent.ReservationName;
    public Dictionary<string, object> Items => _parent.Items;
    public IServiceProvider ServiceProvider => _parent.ServiceProvider;

    public event EventHandler<UnitOfWorkFailedEventArgs>? Failed;
    public event EventHandler<UnitOfWorkEventArgs>? Disposed;

    // ─── Lifecycle (delegated to parent) ─────────────────────────────

    public void SetOuter(IUnitOfWork? outer) => _parent.SetOuter(outer);
    public void Initialize(UnitOfWorkOptions options) => _parent.Initialize(options);
    public void Reserve(string reservationName) => _parent.Reserve(reservationName);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _parent.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// No-op for child UoW — only the outermost parent commits.
    /// </summary>
    public Task CompleteAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _parent.RollbackAsync(cancellationToken);

    public void OnCompleted(Func<Task> handler)
        => _parent.OnCompleted(handler);

    // ─── Domain Events ───────────────────────────────────────────────

    public void AddLocalEvent(UnitOfWorkEventRecord eventRecord)
        => _parent.AddLocalEvent(eventRecord);

    public void AddDistributedEvent(UnitOfWorkEventRecord eventRecord)
        => _parent.AddDistributedEvent(eventRecord);

    // ─── Database / Transaction Registry ─────────────────────────────

    public IDatabaseApi? FindDatabaseApi(string key) => _parent.FindDatabaseApi(key);
    public void AddDatabaseApi(string key, IDatabaseApi api) => _parent.AddDatabaseApi(key, api);
    public IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory) => _parent.GetOrAddDatabaseApi(key, factory);

    public ITransactionApi? FindTransactionApi(string key) => _parent.FindTransactionApi(key);
    public void AddTransactionApi(string key, ITransactionApi api) => _parent.AddTransactionApi(key, api);
    public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory) => _parent.GetOrAddTransactionApi(key, factory);

    // ─── Disposal (no-op for child) ──────────────────────────────────

    public void Dispose() { /* Parent manages lifetime */ }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public override string ToString() => $"[ChildUnitOfWork → {_parent}]";
}
