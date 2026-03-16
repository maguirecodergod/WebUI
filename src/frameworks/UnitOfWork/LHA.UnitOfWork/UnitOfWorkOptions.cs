using System.Data;

namespace LHA.UnitOfWork;

/// <summary>
/// Per-operation options for a unit of work.
/// </summary>
public sealed class UnitOfWorkOptions
{
    /// <summary>
    /// Whether this UoW should use an explicit database transaction.
    /// Default: <c>false</c> (no transaction).
    /// </summary>
    public bool IsTransactional { get; set; }

    /// <summary>
    /// Transaction isolation level. Applied only when <see cref="IsTransactional"/> is <c>true</c>.
    /// If <c>null</c>, the database provider's default isolation level is used.
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Timeout in milliseconds. If <c>null</c>, the provider default is used.
    /// </summary>
    public int? TimeoutMs { get; set; }

    /// <summary>Creates a deep copy of these options.</summary>
    public UnitOfWorkOptions Clone() => new()
    {
        IsTransactional = IsTransactional,
        IsolationLevel = IsolationLevel,
        TimeoutMs = TimeoutMs
    };
}
