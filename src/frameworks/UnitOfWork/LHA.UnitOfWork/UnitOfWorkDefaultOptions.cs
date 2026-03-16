using System.Data;

namespace LHA.UnitOfWork;

/// <summary>
/// Global default options applied to every new unit of work
/// unless explicitly overridden by <see cref="UnitOfWorkOptions"/>.
/// Register via <c>services.Configure&lt;UnitOfWorkDefaultOptions&gt;(…)</c>.
/// </summary>
public sealed class UnitOfWorkDefaultOptions
{
    /// <summary>
    /// Controls whether transactions are used by default.
    /// <list type="bullet">
    ///   <item><b>Auto</b> — the UoW decides based on context (default).</item>
    ///   <item><b>Enabled</b> — always transactional.</item>
    ///   <item><b>Disabled</b> — never transactional.</item>
    /// </list>
    /// </summary>
    public TransactionBehavior TransactionBehavior { get; set; } = TransactionBehavior.Auto;

    /// <summary>Default isolation level when transactions are enabled. <c>null</c> = provider default.</summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>Default timeout in milliseconds. <c>null</c> = no timeout.</summary>
    public int? TimeoutMs { get; set; }

    /// <summary>
    /// Normalises the given <paramref name="options"/> by filling in any <c>null</c>
    /// values from these defaults.
    /// </summary>
    internal UnitOfWorkOptions Normalize(UnitOfWorkOptions options)
    {
        options.IsolationLevel ??= IsolationLevel;
        options.TimeoutMs ??= TimeoutMs;
        return options;
    }

    /// <summary>
    /// Resolves the effective <see cref="UnitOfWorkOptions.IsTransactional"/> flag
    /// when the caller has not explicitly set it.
    /// </summary>
    /// <param name="autoValue">
    /// Value to use when <see cref="TransactionBehavior"/> is <see cref="TransactionBehavior.Auto"/>.
    /// </param>
    public bool ResolveIsTransactional(bool autoValue) => TransactionBehavior switch
    {
        TransactionBehavior.Enabled => true,
        TransactionBehavior.Disabled => false,
        _ => autoValue
    };
}

/// <summary>
/// Controls the default transactional behaviour of the unit of work.
/// </summary>
public enum TransactionBehavior
{
    /// <summary>Automatically decide based on context (e.g. read-only queries → no tx).</summary>
    Auto = 0,

    /// <summary>Always wrap in a transaction.</summary>
    Enabled = 1,

    /// <summary>Never use a transaction.</summary>
    Disabled = 2
}
