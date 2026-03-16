using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.UnitOfWork;

/// <summary>
/// Default <see cref="IUnitOfWork"/> implementation.
/// <para>
/// Maintains dictionaries of <see cref="IDatabaseApi"/> and <see cref="ITransactionApi"/>
/// keyed by provider-specific identifiers. On <see cref="CompleteAsync"/>:
/// <list type="number">
///   <item>Flushes all <see cref="ISupportsSavingChanges"/> database APIs.</item>
///   <item>Publishes local and distributed domain events.</item>
///   <item>Commits all registered transactions.</item>
///   <item>Invokes completion handlers.</item>
/// </list>
/// On disposal without completion, triggers <see cref="Failed"/> and rolls back.
/// </para>
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    public Guid Id { get; } = Guid.NewGuid();

    public UnitOfWorkOptions Options { get; private set; } = default!;

    public IUnitOfWork? Outer { get; private set; }

    public bool IsReserved { get; private set; }

    public bool IsDisposed { get; private set; }

    public bool IsCompleted { get; private set; }

    public string? ReservationName { get; private set; }

    public Dictionary<string, object> Items { get; } = [];

    public IServiceProvider ServiceProvider { get; }

    public event EventHandler<UnitOfWorkFailedEventArgs>? Failed;
    public event EventHandler<UnitOfWorkEventArgs>? Disposed;

    private readonly IUnitOfWorkEventPublisher _eventPublisher;
    private readonly UnitOfWorkDefaultOptions _defaultOptions;
    private readonly ILogger<UnitOfWork> _logger;

    private readonly Dictionary<string, IDatabaseApi> _databaseApis = [];
    private readonly Dictionary<string, ITransactionApi> _transactionApis = [];

    private readonly List<Func<Task>> _completedHandlers = [];
    private readonly List<UnitOfWorkEventRecord> _localEvents = [];
    private readonly List<UnitOfWorkEventRecord> _distributedEvents = [];

    private Exception? _exception;
    private bool _isCompleting;
    private bool _isRolledBack;

    public UnitOfWork(
        IServiceProvider serviceProvider,
        IUnitOfWorkEventPublisher eventPublisher,
        IOptions<UnitOfWorkDefaultOptions> defaultOptions,
        ILogger<UnitOfWork> logger)
    {
        ServiceProvider = serviceProvider;
        _eventPublisher = eventPublisher;
        _defaultOptions = defaultOptions.Value;
        _logger = logger;
    }

    // ─── Lifecycle ───────────────────────────────────────────────────

    public virtual void Initialize(UnitOfWorkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (Options is not null)
            throw new InvalidOperationException($"UnitOfWork {Id} has already been initialized.");

        Options = _defaultOptions.Normalize(options.Clone());
        IsReserved = false;

        _logger.LogDebug("UoW [{UowId}] initialized (tx={IsTransactional})", Id, Options.IsTransactional);
    }

    public virtual void Reserve(string reservationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reservationName);

        ReservationName = reservationName;
        IsReserved = true;

        _logger.LogDebug("UoW [{UowId}] reserved as [{Reservation}]", Id, reservationName);
    }

    public void SetOuter(IUnitOfWork? outer) => Outer = outer;

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledBack) return;

        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsSavingChanges saveable)
            {
                await saveable.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledBack) return;

        PreventMultipleComplete();

        try
        {
            _isCompleting = true;

            // 1. Flush pending changes
            await SaveChangesAsync(cancellationToken);

            // 2. Publish domain events (may trigger additional changes)
            await PublishEventsAsync(cancellationToken);

            // 3. Commit all transactions
            await CommitTransactionsAsync(cancellationToken);

            IsCompleted = true;

            _logger.LogDebug("UoW [{UowId}] completed successfully", Id);

            // 4. Post-commit handlers (notifications, cache invalidation, etc.)
            await InvokeCompletedHandlersAsync();
        }
        catch (Exception ex)
        {
            _exception = ex;
            _logger.LogError(ex, "UoW [{UowId}] completion failed", Id);
            throw;
        }
    }

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledBack) return;
        _isRolledBack = true;

        _logger.LogDebug("UoW [{UowId}] rolling back", Id);

        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsRollback rollbackable)
            {
                try { await rollbackable.RollbackAsync(cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Rollback failed for database API"); }
            }
        }

        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            if (transactionApi is ISupportsRollback rollbackable)
            {
                try { await rollbackable.RollbackAsync(cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "Rollback failed for transaction API"); }
            }
        }
    }

    public void OnCompleted(Func<Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _completedHandlers.Add(handler);
    }

    // ─── Domain Events ───────────────────────────────────────────────

    public void AddLocalEvent(UnitOfWorkEventRecord eventRecord)
    {
        ArgumentNullException.ThrowIfNull(eventRecord);
        _localEvents.Add(eventRecord);
    }

    public void AddDistributedEvent(UnitOfWorkEventRecord eventRecord)
    {
        ArgumentNullException.ThrowIfNull(eventRecord);
        _distributedEvents.Add(eventRecord);
    }

    // ─── Database / Transaction Registry ─────────────────────────────

    public IDatabaseApi? FindDatabaseApi(string key)
        => _databaseApis.GetValueOrDefault(key);

    public void AddDatabaseApi(string key, IDatabaseApi api)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(api);

        if (!_databaseApis.TryAdd(key, api))
            throw new InvalidOperationException($"UoW already contains a database API for key '{key}'.");
    }

    public IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (_databaseApis.TryGetValue(key, out var existing))
            return existing;

        var api = factory();
        _databaseApis[key] = api;
        return api;
    }

    public ITransactionApi? FindTransactionApi(string key)
        => _transactionApis.GetValueOrDefault(key);

    public void AddTransactionApi(string key, ITransactionApi api)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(api);

        if (!_transactionApis.TryAdd(key, api))
            throw new InvalidOperationException($"UoW already contains a transaction API for key '{key}'.");
    }

    public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (_transactionApis.TryGetValue(key, out var existing))
            return existing;

        var api = factory();
        _transactionApis[key] = api;
        return api;
    }

    // ─── Disposal ────────────────────────────────────────────────────

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed) return;

        if (!IsCompleted && _exception is null && !_isRolledBack)
        {
            try { await RollbackAsync(); }
            catch (Exception ex) { _logger.LogWarning(ex, "Rollback during async dispose failed"); }
        }

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        IsDisposed = true;

        if (!disposing) return;

        DisposeTransactions();

        if (!IsCompleted || _exception is not null)
            OnFailed();

        OnDisposed();
    }

    // ─── Internal helpers ────────────────────────────────────────────

    protected IReadOnlyList<IDatabaseApi> GetAllActiveDatabaseApis()
        => _databaseApis.Values.ToImmutableList();

    protected IReadOnlyList<ITransactionApi> GetAllActiveTransactionApis()
        => _transactionApis.Values.ToImmutableList();

    private async Task PublishEventsAsync(CancellationToken cancellationToken)
    {
        // Events may trigger additional SaveChanges + more events, so loop until stable
        while (_localEvents.Count > 0 || _distributedEvents.Count > 0)
        {
            if (_localEvents.Count > 0)
            {
                var batch = _localEvents.OrderBy(e => e.EventOrder).ToList();
                _localEvents.Clear();
                await _eventPublisher.PublishLocalEventsAsync(batch, cancellationToken);
            }

            if (_distributedEvents.Count > 0)
            {
                var batch = _distributedEvents.OrderBy(e => e.EventOrder).ToList();
                _distributedEvents.Clear();
                await _eventPublisher.PublishDistributedEventsAsync(batch, cancellationToken);
            }

            // Event handlers may have flushed more changes / events
            await SaveChangesAsync(cancellationToken);
        }
    }

    private async Task CommitTransactionsAsync(CancellationToken cancellationToken)
    {
        foreach (var transaction in GetAllActiveTransactionApis())
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    private async Task InvokeCompletedHandlersAsync()
    {
        foreach (var handler in _completedHandlers)
        {
            try { await handler(); }
            catch (Exception ex) { _logger.LogWarning(ex, "UoW completion handler threw"); }
        }
    }

    private void DisposeTransactions()
    {
        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            try { transactionApi.Dispose(); }
            catch (Exception ex) { _logger.LogWarning(ex, "Transaction disposal threw"); }
        }
    }

    private void PreventMultipleComplete()
    {
        if (IsCompleted || _isCompleting)
            throw new InvalidOperationException($"UoW {Id} has already been completed or is completing.");
    }

    private void OnFailed()
        => Failed?.Invoke(this, new UnitOfWorkFailedEventArgs(this, _exception, _isRolledBack));

    private void OnDisposed()
        => Disposed?.Invoke(this, new UnitOfWorkEventArgs(this));

    public override string ToString() => $"[UnitOfWork {Id}]";
}
