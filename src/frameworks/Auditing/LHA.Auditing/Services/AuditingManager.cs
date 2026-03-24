using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.Auditing;

/// <summary>
/// Default <see cref="IAuditingManager"/> backed by <see cref="AsyncLocal{T}"/>
/// for ambient scope propagation across async flows.
/// </summary>
internal sealed class AuditingManager : IAuditingManager
{
    private readonly AsyncLocal<AuditLogScope?> _currentScope = new();
    private readonly IAuditingStore _auditingStore;
    private readonly IAuditUserProvider _userProvider;
    private readonly AuditingOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditingManager> _logger;

    public AuditingManager(
        IAuditingStore auditingStore,
        IAuditUserProvider userProvider,
        IOptions<AuditingOptions> options,
        IServiceProvider serviceProvider,
        ILogger<AuditingManager>? logger = null)
    {
        _auditingStore = auditingStore;
        _userProvider = userProvider;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger ?? NullLogger<AuditingManager>.Instance;
    }

    /// <inheritdoc />
    public IAuditLogScope? Current => _currentScope.Value;

    /// <inheritdoc />
    public IAuditLogSaveHandle BeginScope()
    {
        var entry = CreateAuditLogEntry();
        var scope = new AuditLogScope(entry);
        var previous = _currentScope.Value;
        _currentScope.Value = scope;

        return new SaveHandle(this, scope, previous, Stopwatch.StartNew());
    }

    private AuditLogEntry CreateAuditLogEntry()
    {
        var entry = new AuditLogEntry
        {
            ApplicationName = _options.ApplicationName,
            UserId = _userProvider.UserId,
            UserName = _userProvider.UserName,
            TenantId = _userProvider.TenantId,
            ExecutionTime = TimeProvider.System.GetLocalNow()
        };

        // Execute pre-contributors
        ExecuteContributors(entry, preContribute: true);

        return entry;
    }

    private void ExecuteContributors(AuditLogEntry entry, bool preContribute)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = new AuditLogContributionContext(scope.ServiceProvider, entry);

        var contributors = scope.ServiceProvider.GetServices<IAuditLogContributor>();
        foreach (var contributor in contributors)
        {
            try
            {
                if (preContribute)
                    contributor.PreContribute(context);
                else
                    contributor.PostContribute(context);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Audit log contributor {ContributorType} failed.", contributor.GetType().Name);
            }
        }
    }

    private async Task SaveAsync(AuditLogEntry entry, Stopwatch stopwatch)
    {
        stopwatch.Stop();
        entry.ExecutionDuration = (int)stopwatch.ElapsedMilliseconds;

        ExecuteContributors(entry, preContribute: false);
        entry.MergeEntityChanges();

        try
        {
            await _auditingStore.SaveAsync(entry);
        }
        catch (Exception ex)
        {
            if (_options.HideErrors)
            {
                _logger.LogWarning(ex, "Failed to save audit log entry.");
            }
            else
            {
                throw;
            }
        }
    }

    private sealed class AuditLogScope(AuditLogEntry log) : IAuditLogScope
    {
        public AuditLogEntry Log { get; } = log;
    }

    private sealed class SaveHandle(
        AuditingManager manager,
        AuditLogScope scope,
        AuditLogScope? previous,
        Stopwatch stopwatch) : IAuditLogSaveHandle
    {
        private bool _saved;

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (_saved) return;
            _saved = true;
            await manager.SaveAsync(scope.Log, stopwatch);
        }

        public async ValueTask DisposeAsync()
        {
            manager._currentScope.Value = previous;
            if (!_saved)
            {
                await SaveAsync();
            }
        }
    }
}
