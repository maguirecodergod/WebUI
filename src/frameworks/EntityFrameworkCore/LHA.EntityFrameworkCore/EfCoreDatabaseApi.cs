using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// <see cref="IDatabaseApi"/> implementation that wraps a <see cref="DbContext"/>
/// and delegates <see cref="ISupportsSavingChanges.SaveChangesAsync"/> to it.
/// </summary>
public sealed class EfCoreDatabaseApi : IDatabaseApi, ISupportsSavingChanges
{
    /// <summary>
    /// The underlying <see cref="DbContext"/> managed by this database API.
    /// </summary>
    public DbContext DbContext { get; }

    public EfCoreDatabaseApi(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        DbContext = dbContext;
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        DbContext.SaveChangesAsync(cancellationToken);
}
