namespace LHA.UnitOfWork;

/// <summary>
/// Indicates that a <see cref="IDatabaseApi"/> supports flushing pending changes
/// to the underlying store (e.g. EF Core <c>SaveChangesAsync</c>).
/// </summary>
public interface ISupportsSavingChanges
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
