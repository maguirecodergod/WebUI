using LHA.UnitOfWork;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Marker interface for DbContexts that support being bound to a <see cref="IUnitOfWork"/>.
/// Used by <see cref="UnitOfWorkDbContextProvider{TDbContext}"/> to set the current
/// unit of work on the DbContext regardless of its concrete generic type.
/// </summary>
public interface IHasCurrentUnitOfWork
{
    /// <summary>
    /// Gets or sets the current <see cref="IUnitOfWork"/> for this DbContext.
    /// </summary>
    IUnitOfWork? CurrentUnitOfWork { get; set; }
}
