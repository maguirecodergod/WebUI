using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// <see cref="ITransactionApi"/> implementation that wraps an EF Core
/// <see cref="IDbContextTransaction"/> and manages the set of DbContexts
/// participating in the same transaction.
/// </summary>
public sealed class EfCoreTransactionApi : ITransactionApi, ISupportsRollback
{
    /// <summary>
    /// The underlying EF Core transaction.
    /// </summary>
    public IDbContextTransaction Transaction { get; }

    /// <summary>
    /// The DbContext that initiated this transaction.
    /// </summary>
    public DbContext StarterDbContext { get; }

    /// <summary>
    /// Additional DbContexts that have joined this transaction via
    /// <see cref="RelationalDatabaseFacadeExtensions.UseTransaction"/>.
    /// </summary>
    public List<DbContext> AttendedDbContexts { get; } = [];

    public EfCoreTransactionApi(IDbContextTransaction transaction, DbContext starterDbContext)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(starterDbContext);

        Transaction = transaction;
        StarterDbContext = starterDbContext;
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // Save changes from all attended DbContexts first.
        foreach (var dbContext in AttendedDbContexts)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await Transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await Transaction.RollbackAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Transaction.Dispose();
    }
}
