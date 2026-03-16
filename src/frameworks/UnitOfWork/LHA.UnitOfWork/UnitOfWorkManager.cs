using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.UnitOfWork;

/// <summary>
/// Default <see cref="IUnitOfWorkManager"/> implementation.
/// <para>
/// Creates and tracks unit of work instances using <see cref="AmbientUnitOfWork"/>
/// for async-context-aware ambient tracking. Each new root UoW is created in
/// its own DI scope (disposed when the UoW is disposed).
/// </para>
/// </summary>
public sealed class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AmbientUnitOfWork _ambientUnitOfWork;
    private readonly ILogger<UnitOfWorkManager> _logger;

    /// <summary>Well-known reservation name used by middleware.</summary>
    public const string ActionReservationName = "_ActionUnitOfWork";

    public IUnitOfWork? Current => _ambientUnitOfWork.GetCurrentByChecking();

    public UnitOfWorkManager(
        AmbientUnitOfWork ambientUnitOfWork,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UnitOfWorkManager> logger)
    {
        _ambientUnitOfWork = ambientUnitOfWork;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false)
    {
        ArgumentNullException.ThrowIfNull(options);

        var currentUow = Current;
        if (currentUow is not null && !requiresNew)
        {
            return new ChildUnitOfWork(currentUow);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Initialize(options);
        return unitOfWork;
    }

    public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reservationName);

        if (!requiresNew
            && _ambientUnitOfWork.UnitOfWork is not null
            && IsReservedFor(_ambientUnitOfWork.UnitOfWork, reservationName))
        {
            return new ChildUnitOfWork(_ambientUnitOfWork.UnitOfWork);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Reserve(reservationName);
        return unitOfWork;
    }

    public void BeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        if (!TryBeginReserved(reservationName, options))
            throw new InvalidOperationException(
                $"Could not find a reserved unit of work with reservation name: {reservationName}");
    }

    public bool TryBeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reservationName);

        var uow = _ambientUnitOfWork.UnitOfWork;

        // Walk up the outer chain to find the matching reservation
        while (uow is not null && !IsReservedFor(uow, reservationName))
        {
            uow = uow.Outer;
        }

        if (uow is null) return false;

        uow.Initialize(options);
        return true;
    }

    // ─── Internal helpers ────────────────────────────────────────────

    private IUnitOfWork CreateNewUnitOfWork()
    {
        var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var outerUow = _ambientUnitOfWork.UnitOfWork;
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            unitOfWork.SetOuter(outerUow);
            _ambientUnitOfWork.SetUnitOfWork(unitOfWork);

            unitOfWork.Disposed += (_, _) =>
            {
                _ambientUnitOfWork.SetUnitOfWork(outerUow);
                scope.Dispose();
            };

            _logger.LogDebug(
                "Created UoW [{UowId}] (outer={OuterUowId})",
                unitOfWork.Id, outerUow?.Id);

            return unitOfWork;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    private static bool IsReservedFor(IUnitOfWork uow, string reservationName)
        => uow.IsReserved && string.Equals(uow.ReservationName, reservationName, StringComparison.Ordinal);
}
