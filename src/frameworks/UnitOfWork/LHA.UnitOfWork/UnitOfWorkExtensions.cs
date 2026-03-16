using System.Data;

namespace LHA.UnitOfWork;

/// <summary>
/// Extension methods for <see cref="IUnitOfWork"/> and <see cref="IUnitOfWorkManager"/>.
/// </summary>
public static class UnitOfWorkExtensions
{
    // ─── IUnitOfWork extensions ──────────────────────────────────────

    /// <summary>Checks whether the UoW is reserved with the given name.</summary>
    public static bool IsReservedFor(this IUnitOfWork unitOfWork, string reservationName)
        => unitOfWork.IsReserved
           && string.Equals(unitOfWork.ReservationName, reservationName, StringComparison.Ordinal);

    /// <summary>Adds or overwrites an item in the UoW's context bag.</summary>
    public static void SetItem<TValue>(this IUnitOfWork unitOfWork, string key, TValue value)
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        unitOfWork.Items[key] = value;
    }

    /// <summary>Gets a typed item from the UoW's context bag, or <c>null</c> if not found.</summary>
    public static TValue? GetItem<TValue>(this IUnitOfWork unitOfWork, string key)
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Items.TryGetValue(key, out var value) ? value as TValue : null;
    }

    /// <summary>Gets or creates a typed item in the UoW's context bag.</summary>
    public static TValue GetOrAddItem<TValue>(this IUnitOfWork unitOfWork, string key, Func<string, TValue> factory)
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(factory);

        if (unitOfWork.Items.TryGetValue(key, out var existing) && existing is TValue typed)
            return typed;

        var created = factory(key);
        unitOfWork.Items[key] = created;
        return created;
    }

    /// <summary>Removes an item from the UoW's context bag.</summary>
    public static bool RemoveItem(this IUnitOfWork unitOfWork, string key)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Items.Remove(key);
    }

    // ─── IUnitOfWorkManager convenience overloads ────────────────────

    /// <summary>Begins a UoW with default options.</summary>
    public static IUnitOfWork Begin(
        this IUnitOfWorkManager manager,
        bool requiresNew = false,
        bool isTransactional = false,
        IsolationLevel? isolationLevel = null,
        int? timeoutMs = null)
    {
        return manager.Begin(new UnitOfWorkOptions
        {
            IsTransactional = isTransactional,
            IsolationLevel = isolationLevel,
            TimeoutMs = timeoutMs
        }, requiresNew);
    }

    /// <summary>Begins a reserved UoW with default options.</summary>
    public static void BeginReserved(this IUnitOfWorkManager manager, string reservationName)
        => manager.BeginReserved(reservationName, new UnitOfWorkOptions());

    /// <summary>Tries to begin a reserved UoW with default options.</summary>
    public static bool TryBeginReserved(this IUnitOfWorkManager manager, string reservationName)
        => manager.TryBeginReserved(reservationName, new UnitOfWorkOptions());
}
