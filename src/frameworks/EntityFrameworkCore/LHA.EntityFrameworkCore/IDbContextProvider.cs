using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Provides access to a <see cref="DbContext"/> instance scoped to the current unit of work.
/// <para>
/// Returns <see cref="DbContext"/> (not <typeparamref name="TDbContext"/>) to support the
/// ReplaceDbContext pattern — where a unified DbContext (e.g. AccountDbContext) is resolved
/// in place of a module's own context (e.g. IdentityDbContext).
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The logical DbContext type used for resolution lookup.</typeparam>
public interface IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    /// Resolves a <see cref="DbContext"/> for the current scope.
    /// May return a different concrete type if a replacement has been registered
    /// via <see cref="LhaDbContextOptions.ReplaceDbContext{TOriginal, TReplacement}"/>.
    /// </summary>
    Task<DbContext> GetDbContextAsync();
}
