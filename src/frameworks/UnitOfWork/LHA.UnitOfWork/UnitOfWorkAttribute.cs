using System.Data;

namespace LHA.UnitOfWork;

/// <summary>
/// Declares that a method (or all methods of a class) should execute within a unit of work.
/// <para>
/// This attribute carries metadata only — it does not trigger automatic UoW creation.
/// Middleware or filters should inspect this attribute and create UoWs accordingly.
/// </para>
/// </summary>
/// <example>
/// <code>
/// [UnitOfWork(IsTransactional = true, IsolationLevel = IsolationLevel.ReadCommitted)]
/// public async Task CreateOrderAsync(CreateOrderCommand command) { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class UnitOfWorkAttribute : Attribute
{
    /// <summary>
    /// Whether the UoW should be transactional. <c>null</c> = use global default.
    /// </summary>
    public bool? IsTransactional { get; set; }

    /// <summary>Timeout in milliseconds. <c>null</c> = use global default.</summary>
    public int? TimeoutMs { get; set; }

    /// <summary>
    /// Transaction isolation level. <c>null</c> = use global default.
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// If <c>true</c>, disables UoW for this method even if the class has a UoW attribute.
    /// Default: <c>false</c>.
    /// </summary>
    public bool IsDisabled { get; set; }

    public UnitOfWorkAttribute() { }

    public UnitOfWorkAttribute(bool isTransactional)
    {
        IsTransactional = isTransactional;
    }

    /// <summary>
    /// Applies the attribute values to the given <paramref name="options"/>,
    /// overriding only those fields that are explicitly set on this attribute.
    /// </summary>
    public void ApplyTo(UnitOfWorkOptions options)
    {
        if (IsTransactional.HasValue)
            options.IsTransactional = IsTransactional.Value;
        if (TimeoutMs.HasValue)
            options.TimeoutMs = TimeoutMs;
        if (IsolationLevel.HasValue)
            options.IsolationLevel = IsolationLevel;
    }
}
