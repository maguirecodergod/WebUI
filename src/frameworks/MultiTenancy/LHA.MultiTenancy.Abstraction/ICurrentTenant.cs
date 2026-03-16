namespace LHA.MultiTenancy;

/// <summary>
/// Provides access to the current tenant in the ambient context.
/// Thread-safe via <c>AsyncLocal</c>-backed implementations.
/// </summary>
public interface ICurrentTenant
{
    /// <summary>
    /// Whether a tenant context is currently active.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// The current tenant ID, or <see langword="null"/> for the host.
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// The current tenant name, if resolved.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Temporarily switches to a different tenant scope.
    /// Dispose the returned handle to restore the previous scope.
    /// </summary>
    IDisposable Change(Guid? id, string? name = null);
}
